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

    /// <summary>Regla del dominio: no se registra la factura de un mes que todavía no llegó.</summary>
    public static bool EsPeriodoFuturo(int anio, int mes)
    {
        var hoy = DateTime.UtcNow.AddHours(-4); // Bolivia es UTC-4 todo el año
        return anio > hoy.Year || (anio == hoy.Year && mes > hoy.Month);
    }
}
