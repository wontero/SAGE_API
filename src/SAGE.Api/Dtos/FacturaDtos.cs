using System.ComponentModel.DataAnnotations;
using SAGE.Api.Models;

namespace SAGE.Api.Dtos;

/// <summary>Datos para registrar o corregir la factura de energía de un mes.</summary>
public class FacturaEntradaDto
{
    /// <example>4</example>
    [Required(ErrorMessage = "La empresa es obligatoria.")]
    public int? EmpresaId { get; set; }

    /// <example>2026</example>
    [Required(ErrorMessage = "El año es obligatorio.")]
    [Range(2000, 2100, ErrorMessage = "El año debe estar entre 2000 y 2100.")]
    public int? Anio { get; set; }

    /// <example>10</example>
    [Required(ErrorMessage = "El mes es obligatorio.")]
    [Range(1, 12, ErrorMessage = "El mes debe estar entre 1 y 12.")]
    public int? Mes { get; set; }

    /// <example>1750.5</example>
    [Required(ErrorMessage = "El consumo es obligatorio.")]
    [Range(0.001, 1000000000, ErrorMessage = "El consumo (kWh) debe ser mayor que 0.")]
    public decimal? ConsumoKwh { get; set; }

    /// <example>1575.45</example>
    [Required(ErrorMessage = "El monto es obligatorio.")]
    [Range(0.01, 1000000000, ErrorMessage = "El monto (Bs) debe ser mayor que 0.")]
    public decimal? MontoBs { get; set; }
}

/// <summary>Factura devuelta por la API.</summary>
public record FacturaDto(
    int Id,
    int EmpresaId,
    string EmpresaNombre,
    int Anio,
    int Mes,
    string Periodo,
    decimal ConsumoKwh,
    decimal MontoBs,
    decimal TarifaPromedioBsKwh,
    DateTime FechaRegistro)
{
    private static readonly string[] Meses =
    [
        "Enero", "Febrero", "Marzo", "Abril",
        "Mayo", "Junio", "Julio", "Agosto",
        "Septiembre", "Octubre", "Noviembre", "Diciembre"
    ];

    /// <summary>Convierte una factura con su Empresa cargada (Include).</summary>
    public static FacturaDto Desde(Factura f) => new(
        f.Id,
        f.EmpresaId,
        f.Empresa.Nombre,
        f.Anio,
        f.Mes,
        $"{Meses[f.Mes - 1]} {f.Anio}",
        f.ConsumoKwh,
        f.MontoBs,
        Math.Round(f.MontoBs / f.ConsumoKwh, 4),
        f.FechaRegistro
    );
}