using System.ComponentModel.DataAnnotations;

namespace SAGE.Api.Dtos;

/// <summary>
/// Datos necesarios para crear o actualizar una empresa.
/// </summary>
public class EmpresaEntradaDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150, MinimumLength = 2,
        ErrorMessage = "El nombre debe tener entre 2 y 150 caracteres.")]
    public string? Nombre { get; set; }

    [Required(ErrorMessage = "El NIT es obligatorio.")]
    [RegularExpression(@"^\d{5,20}$",
        ErrorMessage = "El NIT debe tener de 5 a 20 dígitos, sin letras ni espacios.")]
    public string? Nit { get; set; }

    [StringLength(250,
        ErrorMessage = "La dirección no puede superar 250 caracteres.")]
    public string? Direccion { get; set; }

    public bool Activa { get; set; } = true;
}

/// <summary>
/// Datos de una empresa que devuelve la API.
/// </summary>
public record EmpresaDto(
    int Id,
    string Nombre,
    string Nit,
    string? Direccion,
    bool Activa,
    DateTime FechaRegistro,
    int CantidadFacturas
);