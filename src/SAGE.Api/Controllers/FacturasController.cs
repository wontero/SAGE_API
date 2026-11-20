using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAGE.Api.Data;
using SAGE.Api.Dtos;
using SAGE.Api.Models;

namespace SAGE.Api.Controllers;

[ApiController]
[Route("api/facturas")]
[Produces("application/json")]
public class FacturasController(SageApiDbContext context) : ControllerBase
{
    /// <summary>Obtiene las facturas ordenadas por empresa, año y mes.</summary>
    /// <response code="200">Lista de facturas.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<FacturaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FacturaDto>>> Listar([FromQuery] int? empresaId)
    {
        var consulta = ConsultarFacturas();
        if (empresaId.HasValue)
        {
            consulta = consulta.Where(f => f.EmpresaId == empresaId);
        }

        var facturas = await consulta
            .OrderBy(f => f.EmpresaId)
            .ThenBy(f => f.Anio)
            .ThenBy(f => f.Mes)
            .ToListAsync();

        return Ok(facturas.Select(FacturaDto.Desde));
    }

    /// <summary>Obtiene una factura por su identificador.</summary>
    /// <response code="200">Factura encontrada.</response>
    /// <response code="404">La factura no existe.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(FacturaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FacturaDto>> ObtenerPorId(int id)
    {
        var factura = await ConsultarFacturas().FirstOrDefaultAsync(f => f.Id == id);
        if (factura is null)
        {
            return FacturaNoEncontrada(id);
        }

        return Ok(FacturaDto.Desde(factura));
    }

    /// <summary>Registra la factura de energía de una empresa para un mes.</summary>
    /// <response code="201">Factura creada.</response>
    /// <response code="400">Datos no válidos, empresa inexistente o inactiva, o periodo futuro.</response>
    /// <response code="409">La empresa ya tiene una factura para ese periodo.</response>
    [HttpPost]
    [ProducesResponseType(typeof(FacturaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FacturaDto>> Crear([FromBody] FacturaEntradaDto dto)
    {
        var error = await ValidarPeriodo(dto, id: null);
        if (error is not null)
        {
            return error;
        }

        var factura = new Factura
        {
            EmpresaId = dto.EmpresaId!.Value,
            Anio = dto.Anio!.Value,
            Mes = dto.Mes!.Value,
            ConsumoKwh = dto.ConsumoKwh!.Value,
            MontoBs = dto.MontoBs!.Value
        };
        context.Facturas.Add(factura);

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return PeriodoRepetido(dto);
        }

        var id = factura.Id;
        var guardada = await ConsultarFacturas().FirstAsync(f => f.Id == id);
        return CreatedAtAction(nameof(ObtenerPorId), new { id }, FacturaDto.Desde(guardada));
    }

    /// <summary>Actualiza los datos de una factura existente.</summary>
    /// <response code="200">Factura actualizada.</response>
    /// <response code="400">Datos no válidos, empresa inexistente o inactiva, o periodo futuro.</response>
    /// <response code="404">La factura no existe.</response>
    /// <response code="409">La empresa ya tiene otra factura para ese periodo.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(FacturaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FacturaDto>> Actualizar(int id, [FromBody] FacturaEntradaDto dto)
    {
        var factura = await context.Facturas.FindAsync(id);
        if (factura is null)
        {
            return FacturaNoEncontrada(id);
        }

        var error = await ValidarPeriodo(dto, id);
        if (error is not null)
        {
            return error;
        }

        factura.EmpresaId = dto.EmpresaId!.Value;
        factura.Anio = dto.Anio!.Value;
        factura.Mes = dto.Mes!.Value;
        factura.ConsumoKwh = dto.ConsumoKwh!.Value;
        factura.MontoBs = dto.MontoBs!.Value;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return PeriodoRepetido(dto);
        }

        var guardada = await ConsultarFacturas().FirstAsync(f => f.Id == id);
        return Ok(FacturaDto.Desde(guardada));
    }

    /// <summary>Elimina una factura por su identificador.</summary>
    /// <response code="204">Factura eliminada.</response>
    /// <response code="404">La factura no existe.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(int id)
    {
        var factura = await context.Facturas.FindAsync(id);
        if (factura is null)
        {
            return FacturaNoEncontrada(id);
        }

        context.Facturas.Remove(factura);
        await context.SaveChangesAsync();
        return NoContent();
    }

    private IQueryable<Factura> ConsultarFacturas() =>
        context.Facturas.AsNoTracking().Include(f => f.Empresa);

    private async Task<ActionResult?> ValidarPeriodo(FacturaEntradaDto dto, int? id)
    {
        var empresaId = dto.EmpresaId!.Value;
        var empresa = await context.Empresas.AsNoTracking().FirstOrDefaultAsync(e => e.Id == empresaId);
        if (empresa is null)
        {
            ModelState.AddModelError("EmpresaId", $"La empresa {empresaId} no existe.");
            return ValidationProblem(ModelState);
        }

        if (!empresa.Activa)
        {
            ModelState.AddModelError("EmpresaId", $"La empresa {empresaId} está inactiva y no admite facturas.");
            return ValidationProblem(ModelState);
        }

        var anio = dto.Anio!.Value;
        var mes = dto.Mes!.Value;
        if (Factura.EsPeriodoFuturo(anio, mes))
        {
            ModelState.AddModelError("Mes", $"El periodo {mes}/{anio} es futuro: solo se registran meses ya facturados.");
            return ValidationProblem(ModelState);
        }

        var repetido = await context.Facturas.AnyAsync(f =>
            f.EmpresaId == empresaId && f.Anio == anio && f.Mes == mes && f.Id != id);
        return repetido ? PeriodoRepetido(dto) : null;
    }

    private ObjectResult FacturaNoEncontrada(int id) =>
        Problem(
            statusCode: StatusCodes.Status404NotFound,
            title: "Factura no encontrada",
            detail: $"No existe la factura con id {id}.");

    private ObjectResult PeriodoRepetido(FacturaEntradaDto dto) =>
        Problem(
            statusCode: StatusCodes.Status409Conflict,
            title: "Periodo repetido",
            detail: $"La empresa {dto.EmpresaId} ya tiene la factura de {dto.Mes}/{dto.Anio}.");
}
