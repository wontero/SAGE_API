using Microsoft.EntityFrameworkCore;
using SAGE.Api.Models;

namespace SAGE.Api.Data;

public class SageApiDbContext(DbContextOptions<SageApiDbContext> options) : DbContext(options)
{
    public DbSet<Empresa> Empresas => Set<Empresa>();
    public DbSet<Factura> Facturas => Set<Factura>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var empresas = modelBuilder.Entity<Empresa>();
        empresas.Property(e => e.Nombre).IsRequired().HasMaxLength(150);
        empresas.Property(e => e.Nit).IsRequired().HasMaxLength(20);
        empresas.Property(e => e.Direccion).HasMaxLength(250);
        empresas.HasIndex(e => e.Nit).IsUnique();

        var facturas = modelBuilder.Entity<Factura>();
        facturas.HasIndex(f => new { f.EmpresaId, f.Anio, f.Mes }).IsUnique();
        facturas.Property(f => f.ConsumoKwh).HasPrecision(18, 3);
        facturas.Property(f => f.MontoBs).HasPrecision(18, 2);
        facturas.HasOne(f => f.Empresa)
            .WithMany(e => e.Facturas)
            .HasForeignKey(f => f.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);
        facturas.ToTable(t => t.HasCheckConstraint("CK_Facturas_Mes", "[Mes] BETWEEN 1 AND 12"));
    }
}
