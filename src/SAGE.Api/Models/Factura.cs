namespace SAGE.Api.Models;

public class Factura
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public Empresa Empresa { get; set; } = null!;
    public int Anio { get; set; }
    public int Mes { get; set; }
    public decimal ConsumoKwh { get; set; }
    public decimal MontoBs { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
}
