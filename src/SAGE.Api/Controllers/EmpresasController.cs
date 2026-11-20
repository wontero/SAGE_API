using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAGE.Api.Data;
using SAGE.Api.Dtos;
using SAGE.Api.Models;

namespace SAGE.Api.Controllers;

[ApiController]
[Route("api/empresas")]
[Produces("application/json")]
public class EmpresasController : ControllerBase
{
    private readonly SageApiDbContext _context;

    public EmpresasController(SageApiDbContext context)
    {
        _context = context;
    }

    /// <summary>Obtiene todas las empresas ordenadas por nombre.</summary>
    /// <response code="200">Lista de empresas.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EmpresaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EmpresaDto>>> Listar()
    {
        var empresas = await _context.Empresas
            .AsNoTracking()
            .OrderBy(e => e.Nombre)
            .Select(e => new EmpresaDto(
                e.Id,
                e.Nombre,
                e.Nit,
                e.Direccion,
                e.Activa,
                e.FechaRegistro,
                e.Facturas.Count))
            .ToListAsync();

        return Ok(empresas);
    }

    /// <summary>Obtiene una empresa por su identificador.</summary>
    /// <response code="200">Empresa encontrada.</response>
    /// <response code="404">La empresa no existe.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EmpresaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmpresaDto>> ObtenerPorId(int id)
    {
        var empresa = await _context.Empresas
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(e => new EmpresaDto(
                e.Id,
                e.Nombre,
                e.Nit,
                e.Direccion,
                e.Activa,
                e.FechaRegistro,
                e.Facturas.Count))
            .FirstOrDefaultAsync();

        if (empresa is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Empresa no encontrada",
                detail: $"No existe la empresa con id {id}.");
        }

        return Ok(empresa);
    }

    /// <summary>Obtiene las facturas de una empresa.</summary>
    /// <response code="200">Lista de facturas.</response>
    /// <response code="404">La empresa no existe.</response>
    [HttpGet("{id:int}/facturas")]
    [ProducesResponseType(typeof(IEnumerable<FacturaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<FacturaDto>>> ListarFacturas(int id)
    {
        var existe = await _context.Empresas
            .AnyAsync(e => e.Id == id);

        if (!existe)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Empresa no encontrada",
                detail: $"No existe la empresa con id {id}.");
        }

        var facturas = await _context.Facturas
            .AsNoTracking()
            .Where(f => f.EmpresaId == id)
            .Include(f => f.Empresa)
            .OrderBy(f => f.Anio)
            .ThenBy(f => f.Mes)
            .ToListAsync();

        var resultado = facturas
            .Select(FacturaDto.Desde)
            .ToList();

        return Ok(resultado);
    }

    /// <summary>Registra una nueva empresa.</summary>
    /// <response code="201">Empresa creada.</response>
    /// <response code="400">Datos no válidos.</response>
    /// <response code="409">NIT repetido.</response>
    [HttpPost]
    [ProducesResponseType(typeof(EmpresaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<EmpresaDto>> Crear(
        [FromBody] EmpresaEntradaDto dto)
    {
        var nombre = dto.Nombre!.Trim();
        var nit = dto.Nit!.Trim();

        var nitExiste = await _context.Empresas
            .AnyAsync(e => e.Nit == nit);

        if (nitExiste)
        {
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "NIT repetido",
                detail: $"Ya existe una empresa con el NIT {nit}.");
        }

        var empresa = new Empresa
        {
            Nombre = nombre,
            Nit = nit,
            Direccion = dto.Direccion,
            Activa = dto.Activa
        };

        _context.Empresas.Add(empresa);
        await _context.SaveChangesAsync();

        var resultado = new EmpresaDto(
            empresa.Id,
            empresa.Nombre,
            empresa.Nit,
            empresa.Direccion,
            empresa.Activa,
            empresa.FechaRegistro,
            0);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = empresa.Id },
            resultado);
    }

    /// <summary>Actualiza una empresa existente.</summary>
    /// <response code="200">Empresa actualizada.</response>
    /// <response code="400">Datos no válidos.</response>
    /// <response code="404">La empresa no existe.</response>
    /// <response code="409">NIT repetido.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(EmpresaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<EmpresaDto>> Actualizar(
        int id,
        [FromBody] EmpresaEntradaDto dto)
    {
        var empresa = await _context.Empresas
            .FirstOrDefaultAsync(e => e.Id == id);

        if (empresa is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Empresa no encontrada",
                detail: $"No existe la empresa con id {id}.");
        }

        var nombre = dto.Nombre!.Trim();
        var nit = dto.Nit!.Trim();

        var nitExiste = await _context.Empresas
            .AnyAsync(e => e.Nit == nit && e.Id != id);

        if (nitExiste)
        {
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "NIT repetido",
                detail: $"Ya existe una empresa con el NIT {nit}.");
        }

        empresa.Nombre = nombre;
        empresa.Nit = nit;
        empresa.Direccion = dto.Direccion;
        empresa.Activa = dto.Activa;

        await _context.SaveChangesAsync();

        var resultado = await _context.Empresas
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(e => new EmpresaDto(
                e.Id,
                e.Nombre,
                e.Nit,
                e.Direccion,
                e.Activa,
                e.FechaRegistro,
                e.Facturas.Count))
            .FirstAsync();

        return Ok(resultado);
    }

    /// <summary>Elimina una empresa que no tenga facturas.</summary>
    /// <response code="204">Empresa eliminada.</response>
    /// <response code="404">La empresa no existe.</response>
    /// <response code="409">La empresa tiene facturas.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Eliminar(int id)
    {
        var empresa = await _context.Empresas
            .FirstOrDefaultAsync(e => e.Id == id);

        if (empresa is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Empresa no encontrada",
                detail: $"No existe la empresa con id {id}.");
        }

        var cantidadFacturas = await _context.Facturas
            .CountAsync(f => f.EmpresaId == id);

        if (cantidadFacturas > 0)
        {
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Empresa con facturas",
                detail: $"La empresa {id} tiene {cantidadFacturas} facturas. Elimínalas antes de borrar la empresa.");
        }

        _context.Empresas.Remove(empresa);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}