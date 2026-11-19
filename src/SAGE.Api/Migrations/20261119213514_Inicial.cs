using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SAGE.Api.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Empresas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Nit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Activa = table.Column<bool>(type: "bit", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Facturas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpresaId = table.Column<int>(type: "int", nullable: false),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    Mes = table.Column<int>(type: "int", nullable: false),
                    ConsumoKwh = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    MontoBs = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facturas", x => x.Id);
                    table.CheckConstraint("CK_Facturas_Mes", "[Mes] BETWEEN 1 AND 12");
                    table.ForeignKey(
                        name: "FK_Facturas_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Empresas",
                columns: new[] { "Id", "Activa", "Direccion", "FechaRegistro", "Nit", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "Av. Cristo Redentor 2450, Santa Cruz", new DateTime(2026, 7, 1, 12, 0, 0, 0, DateTimeKind.Utc), "1023456027", "Energía Andina S.R.L." },
                    { 2, true, "Calle Libertad 120, Santa Cruz", new DateTime(2026, 7, 1, 12, 0, 0, 0, DateTimeKind.Utc), "2034567018", "Hotel Las Palmas" },
                    { 3, true, "Av. Beni 890, Santa Cruz", new DateTime(2026, 7, 1, 12, 0, 0, 0, DateTimeKind.Utc), "3045678019", "Panadería El Trigal" }
                });

            migrationBuilder.InsertData(
                table: "Facturas",
                columns: new[] { "Id", "Anio", "ConsumoKwh", "EmpresaId", "FechaRegistro", "Mes", "MontoBs" },
                values: new object[,]
                {
                    { 1, 2026, 2150.000m, 1, new DateTime(2026, 7, 1, 12, 0, 0, 0, DateTimeKind.Utc), 1, 1935.00m },
                    { 2, 2026, 2080.500m, 1, new DateTime(2026, 7, 1, 12, 0, 0, 0, DateTimeKind.Utc), 2, 1872.45m },
                    { 3, 2026, 2210.000m, 1, new DateTime(2026, 7, 1, 12, 0, 0, 0, DateTimeKind.Utc), 3, 1989.00m },
                    { 4, 2026, 1985.250m, 1, new DateTime(2026, 7, 1, 12, 0, 0, 0, DateTimeKind.Utc), 4, 1786.73m },
                    { 5, 2026, 1890.000m, 1, new DateTime(2026, 7, 1, 12, 0, 0, 0, DateTimeKind.Utc), 5, 1701.00m },
                    { 6, 2026, 1820.750m, 1, new DateTime(2026, 7, 1, 12, 0, 0, 0, DateTimeKind.Utc), 6, 1638.68m },
                    { 7, 2026, 3420.000m, 2, new DateTime(2026, 7, 1, 12, 0, 0, 0, DateTimeKind.Utc), 1, 3078.00m },
                    { 8, 2026, 3315.500m, 2, new DateTime(2026, 7, 1, 12, 0, 0, 0, DateTimeKind.Utc), 2, 2983.95m },
                    { 9, 2026, 3502.000m, 2, new DateTime(2026, 7, 1, 12, 0, 0, 0, DateTimeKind.Utc), 3, 3151.80m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_Nit",
                table: "Empresas",
                column: "Nit",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_EmpresaId_Anio_Mes",
                table: "Facturas",
                columns: new[] { "EmpresaId", "Anio", "Mes" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Facturas");

            migrationBuilder.DropTable(
                name: "Empresas");
        }
    }
}
