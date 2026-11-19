using Microsoft.EntityFrameworkCore;
using SAGE.Api.Models;

namespace SAGE.Api.Data;

public static class DatosSemilla
{
    public static void Cargar(ModelBuilder modelBuilder)
    {
        var fecha = new DateTime(2026, 7, 1, 12, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<Empresa>().HasData(
            new Empresa { Id = 1, Nombre = "Energía Andina S.R.L.", Nit = "1023456027", Direccion = "Av. Cristo Redentor 2450, Santa Cruz", Activa = true, FechaRegistro = fecha },
            new Empresa { Id = 2, Nombre = "Hotel Las Palmas", Nit = "2034567018", Direccion = "Calle Libertad 120, Santa Cruz", Activa = true, FechaRegistro = fecha },
            new Empresa { Id = 3, Nombre = "Panadería El Trigal", Nit = "3045678019", Direccion = "Av. Beni 890, Santa Cruz", Activa = true, FechaRegistro = fecha });

        modelBuilder.Entity<Factura>().HasData(
            new Factura { Id = 1, EmpresaId = 1, Anio = 2026, Mes = 1, ConsumoKwh = 2150.000m, MontoBs = 1935.00m, FechaRegistro = fecha },
            new Factura { Id = 2, EmpresaId = 1, Anio = 2026, Mes = 2, ConsumoKwh = 2080.500m, MontoBs = 1872.45m, FechaRegistro = fecha },
            new Factura { Id = 3, EmpresaId = 1, Anio = 2026, Mes = 3, ConsumoKwh = 2210.000m, MontoBs = 1989.00m, FechaRegistro = fecha },
            new Factura { Id = 4, EmpresaId = 1, Anio = 2026, Mes = 4, ConsumoKwh = 1985.250m, MontoBs = 1786.73m, FechaRegistro = fecha },
            new Factura { Id = 5, EmpresaId = 1, Anio = 2026, Mes = 5, ConsumoKwh = 1890.000m, MontoBs = 1701.00m, FechaRegistro = fecha },
            new Factura { Id = 6, EmpresaId = 1, Anio = 2026, Mes = 6, ConsumoKwh = 1820.750m, MontoBs = 1638.68m, FechaRegistro = fecha },
            new Factura { Id = 7, EmpresaId = 2, Anio = 2026, Mes = 1, ConsumoKwh = 3420.000m, MontoBs = 3078.00m, FechaRegistro = fecha },
            new Factura { Id = 8, EmpresaId = 2, Anio = 2026, Mes = 2, ConsumoKwh = 3315.500m, MontoBs = 2983.95m, FechaRegistro = fecha },
            new Factura { Id = 9, EmpresaId = 2, Anio = 2026, Mes = 3, ConsumoKwh = 3502.000m, MontoBs = 3151.80m, FechaRegistro = fecha });
    }
}
