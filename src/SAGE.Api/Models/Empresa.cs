namespace SAGE.Api.Models;

public class Empresa
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Nit { get; set; } = null!;
    public string? Direccion { get; set; }
    public bool Activa { get; set; } = true;
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    public ICollection<Factura> Facturas { get; set; } = new List<Factura>();
}
