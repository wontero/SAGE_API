# Guía de trabajo — SAGE API (Web API con EF Core y SQL Server)

> **Para la persona:** dale este archivo completo a tu agente de IA y dile:
> *"Lee GUIA_TRABAJO_API.md completo y ejecuta las misiones de &lt;tu nombre&gt;."*
> Tú haces solo la sección 1 (preparar la PC). Al final revisas que el agente marcó sus casillas, escribió en la Bitácora y subió todo **con tu usuario de GitHub**.

> **Para el agente:** esta guía está escrita para ti. Los nombres de proyectos, carpetas, clases, propiedades, rutas, mensajes y códigos HTTP son **exactos**: respeta mayúsculas, minúsculas y tildes. Si algo no está escrito acá, **no lo inventes**: aplica la regla 8.

- **Repositorio:** https://github.com/wontero/SAGE_API (rama `main`)
- **Paso 0 (Mateo, antes de todo):** crear el repositorio vacío en GitHub, subir esta guía como primer commit y agregar a Harold, Matías y Ethan como colaboradores.
- **Orden (estricto):** 1) **Mateo** → 2) **Matías** → 3) **Ethan** → 4) **Harold**. (Mateo y Harold intercambiaron sus partes el 19/11.) Nadie empieza hasta que el anterior subió su trabajo, marcó sus casillas y escribió su entrada en la Bitácora.
- **Qué es:** una Web API REST sobre una parte del dominio de SAGE: **empresas** y sus **facturas de energía** (consumo facturado por mes). Es un proyecto **independiente** de `SAGE-app`: su propio repositorio, su propia base `SAGE_API` y sus propias migraciones. No toca la base `SAGE` ni el código de `SAGE-app`.
- **Entrega:** un ZIP del repositorio (sin `bin`, `obj`, `.vs`) con README y PDF de evidencias. La nota incluye **15 puntos por commits propios**: cada integrante sube sus commits desde su PC y con su usuario.

---

## 0. Reglas para el agente (obligatorias)

1. Trabaja en `main`. Antes de empezar: `git pull`. Al terminar cada misión: `dotnet build` sin errores, **un commit por misión** y `git push`.
2. Mensaje de commit: `API-M<n>: <qué hiciste> (<Nombre>)`. Ejemplo: `API-M4: EmpresasController con CRUD y facturas de la empresa (Matías)`.
3. **El commit debe quedar a nombre de la persona.** Antes del primer commit ejecuta `git config user.name` y `git config user.email` y muéstraselos a la persona: el correo tiene que estar verificado en su cuenta de GitHub, si no el commit no le cuenta. Nunca hagas commits a nombre de otro integrante.
4. Nunca uses `git push --force`, no reescribas historial y no subas `bin/`, `obj/`, `.vs/`, `*.user`, el ZIP ni contraseñas reales (ni en `appsettings*.json`, ni en README, ni en esta guía). La cadena de conexión con usuario y contraseña va en **user-secrets**.
5. Todo acceso a datos es **asíncrono** (`ToListAsync`, `FindAsync`, `FirstOrDefaultAsync`, `AnyAsync`, `SaveChangesAsync`) y el `SageApiDbContext` llega **por constructor** (inyección de dependencias). Prohibido `new SageApiDbContext()`, `.Result` y `.Wait()`.
6. Los controladores **nunca** reciben ni devuelven entidades (`Empresa`, `Factura`): solo DTO (sección 4).
7. Formato de errores: 400 con `ValidationProblem(ModelState)`; 404 y 409 con `Problem(statusCode: ..., title: ..., detail: ...)`. Los textos están en las tablas de cada misión.
8. Si algo no está en la guía o no funciona como dice: elige la opción más simple que cumpla la rúbrica (sección 3), anótalo en la Bitácora como **Desvío** y sigue. Si te impide terminar, anótalo como **BLOQUEO**, sube lo que tengas y detente.
9. No agregues paquetes, autenticación, AutoMapper, repositorios genéricos ni capas extra. La estructura es la de la sección 2 y nada más.
10. Al terminar: marca tus casillas `[x]` en esta guía, escribe tu entrada en la Bitácora (sección 9) y súbela en el último commit.

---

## 1. Preparación de la PC (la hace la persona)

1. SDK de **.NET 10** (`dotnet --list-sdks` debe mostrar una `10.0.x`). Si también tienes .NET 11, no pasa nada: el `global.json` fija la 10.
2. SQL Server: en Windows, **SQL Server Express** (`.\SQLEXPRESS`); en Mac, el contenedor de Docker que ya usas para SAGE.
3. Herramienta de EF Core: `dotnet tool install --global dotnet-ef` (o `dotnet tool update --global dotnet-ef`).
4. Git configurado con **tu** nombre y **tu** correo de GitHub:
   ```
   git config --global user.name "Tu Nombre"
   git config --global user.email "correo-de-tu-cuenta-github@ejemplo.com"
   ```
5. Acceso de escritura al repositorio (Mateo los agrega como colaboradores; acepten la invitación que llega por correo).
6. Clonar: `git clone https://github.com/wontero/SAGE_API.git` y abrir la carpeta con el agente.

---

## 2. Stack y estructura final

| Pieza | Valor |
|---|---|
| Framework | ASP.NET Core Web API con **controladores**, .NET 10 (`net10.0`) |
| ORM | EF Core **10.0.11** (`Microsoft.EntityFrameworkCore.SqlServer`, `.Design`, `.Tools`) |
| Swagger | `Swashbuckle.AspNetCore` (última versión estable; anótala en el README) |
| Base | SQL Server, base **`SAGE_API`**, cadena `ConnectionStrings:SageApi` |
| URL | `http://localhost:5080` (Swagger en `http://localhost:5080/swagger`) |

```
SAGE_API/
├─ .gitignore                 ← dotnet new gitignore (+ línea SAGE_API.zip)
├─ .gitattributes             ← excluye esta guía del ZIP
├─ global.json                ← fija el SDK 10
├─ SAGE.Api.slnx
├─ README.md
├─ GUIA_TRABAJO_API.md        ← esta guía
├─ scripts/empaquetar.ps1     ← genera el ZIP (Harold)
├─ docs/evidencias/           ← capturas, evidencias.html y Evidencias_SAGE_API.pdf (Harold)
└─ src/SAGE.Api/
   ├─ Controllers/            ← EmpresasController.cs (Matías), FacturasController.cs (Ethan)
   ├─ Data/                   ← SageApiDbContext.cs, DatosSemilla.cs (Mateo)
   ├─ Migrations/             ← migración Inicial (Mateo)
   ├─ Dtos/                   ← EmpresaDtos.cs, FacturaDtos.cs (Matías)
   ├─ Models/                 ← Empresa.cs, Factura.cs (Mateo; Ethan agrega una regla)
   ├─ Properties/launchSettings.json
   ├─ appsettings.json
   ├─ Program.cs
   └─ SAGE.Api.http           ← todas las pruebas E01–E23 (Ethan)
```

---

## 3. Rúbrica → dónde se cumple

| Criterio (puntos) | Dónde |
|---|---|
| Entidades, claves y relación (8) | M1: `Empresa` 1—N `Factura`, PK, FK, índices únicos, check |
| Configuración EF Core y migración aplicable (8) | M1–M2: Fluent API, migración `Inicial` |
| Datos de prueba y persistencia tras reiniciar (4) | M2 (semilla en la migración), M7 (prueba E20) |
| Listado y consulta por ID (6) | M4 y M5: `GET /api/empresas`, `GET /api/facturas`, `/{id}` |
| POST con 201 y Location (6) | M4 y M5: `CreatedAtAction` |
| PUT y DELETE funcionales (9) | M4 y M5 |
| Consulta del recurso relacionado (4) | M4: `GET /api/empresas/{id}/facturas` |
| Separación modelos / controladores / DTO / datos (3) | sección 2 |
| DTO de entrada y salida (5) | M3 |
| Inyección del contexto y operaciones asíncronas (2) | regla 5 |
| Campos obligatorios y regla del dominio (5) | M3 (DataAnnotations), M5 (periodo no futuro, un periodo por empresa) |
| Referencias inválidas, 400 y 404 (5) | M5 (`empresaId` inexistente → 400), 404 en todos los `{id}` |
| Swagger con rutas, parámetros, esquemas y respuestas (8) | M2, M4, M5 (`ProducesResponseType` y comentarios XML), M7 |
| Evidencias de éxitos, errores y persistencia (7) | M7: PDF con E01–E23 |
| ZIP completo y limpio (3), README reproducible (2) | M8 |
| Participación en Git (15) | reglas 2 y 3: cada uno sus commits |

---

## 4. Modelo y contrato (lo implementan Mateo y Matías; los demás lo usan tal cual)

**Entidades** (`Models/`):

| Entidad | Propiedad | Tipo | Reglas |
|---|---|---|---|
| `Empresa` | `Id` | `int` | PK, identidad |
| | `Nombre` | `string` | obligatorio, máx. 150 |
| | `Nit` | `string` | obligatorio, máx. 20, **único** |
| | `Direccion` | `string?` | máx. 250 |
| | `Activa` | `bool` | por defecto `true` |
| | `FechaRegistro` | `DateTime` | UTC, la pone el servidor |
| | `Facturas` | `ICollection<Factura>` | navegación |
| `Factura` | `Id` | `int` | PK, identidad |
| | `EmpresaId` | `int` | FK → `Empresa.Id`, `OnDelete(Restrict)` |
| | `Empresa` | `Empresa` | navegación |
| | `Anio` | `int` | 2000–2100 |
| | `Mes` | `int` | 1–12 (check `CK_Facturas_Mes`) |
| | `ConsumoKwh` | `decimal` | precisión (18,3), > 0 |
| | `MontoBs` | `decimal` | precisión (18,2), > 0 |
| | `FechaRegistro` | `DateTime` | UTC, la pone el servidor |

Índices: `IX_Empresas_Nit` único; `IX_Facturas_EmpresaId_Anio_Mes` único (una factura por empresa y mes).

**Rutas** (todas devuelven JSON; `{id}` es entero):

| Método y ruta | Éxito | Errores |
|---|---|---|
| `GET /api/empresas` | 200 lista de `EmpresaDto` (orden por `Nombre`) | — |
| `GET /api/empresas/{id}` | 200 `EmpresaDto` | 404 |
| `GET /api/empresas/{id}/facturas` | 200 lista de `FacturaDto` (orden `Anio`, `Mes`) | 404 si la empresa no existe |
| `POST /api/empresas` | 201 + `Location` + `EmpresaDto` | 400, 409 NIT repetido |
| `PUT /api/empresas/{id}` | 200 `EmpresaDto` actualizado | 400, 404, 409 NIT repetido |
| `DELETE /api/empresas/{id}` | 204 | 404, 409 si tiene facturas |
| `GET /api/facturas?empresaId=` | 200 lista de `FacturaDto` (orden `EmpresaId`, `Anio`, `Mes`; filtro opcional) | — |
| `GET /api/facturas/{id}` | 200 `FacturaDto` | 404 |
| `POST /api/facturas` | 201 + `Location` + `FacturaDto` | 400 (obligatorios, rango, periodo futuro, empresa inexistente o inactiva), 409 periodo repetido |
| `PUT /api/facturas/{id}` | 200 `FacturaDto` actualizado | 400, 404, 409 periodo repetido |
| `DELETE /api/facturas/{id}` | 204 | 404 |

---

## 5. Mateo — proyecto, modelo, base y Swagger

### - [x] API-M1 · Solución, proyecto y modelo

1. En la raíz del repo:
   ```
   dotnet new globaljson --sdk-version 10.0.100 --roll-forward latestFeature
   dotnet new gitignore
   dotnet new sln -n SAGE.Api
   dotnet new webapi --use-controllers --framework net10.0 -n SAGE.Api -o src/SAGE.Api
   dotnet sln add src/SAGE.Api
   ```
   Agrega al final de `.gitignore` la línea `SAGE_API.zip`. Crea `.gitattributes` con la línea `GUIA_TRABAJO_API.md export-ignore`.
2. Borra `WeatherForecast.cs` y `Controllers/WeatherForecastController.cs`. Quita el paquete `Microsoft.AspNetCore.OpenApi` y las líneas `AddOpenApi()` / `MapOpenApi()` de `Program.cs` (usamos Swashbuckle).
3. Paquetes (desde `src/SAGE.Api`):
   ```
   dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 10.0.11
   dotnet add package Microsoft.EntityFrameworkCore.Design --version 10.0.11
   dotnet add package Microsoft.EntityFrameworkCore.Tools --version 10.0.11
   dotnet add package Swashbuckle.AspNetCore
   dotnet user-secrets init
   ```
4. En `SAGE.Api.csproj`, dentro del `PropertyGroup`: `<GenerateDocumentationFile>true</GenerateDocumentationFile>` y `<NoWarn>$(NoWarn);1591</NoWarn>`.
5. `Models/Empresa.cs` y `Models/Factura.cs` según la tabla de la sección 4 (namespace `SAGE.Api.Models`, `FechaRegistro = DateTime.UtcNow` por defecto, `Facturas = new List<Factura>()`).
6. `Data/SageApiDbContext.cs` (namespace `SAGE.Api.Data`), con `DbSet<Empresa> Empresas` y `DbSet<Factura> Facturas`. En `OnModelCreating`, con Fluent API: longitudes y obligatorios de la tabla, `HasIndex(e => e.Nit).IsUnique()`, `HasIndex(f => new { f.EmpresaId, f.Anio, f.Mes }).IsUnique()`, `HasPrecision(18, 3)` y `HasPrecision(18, 2)`, la relación `HasOne(f => f.Empresa).WithMany(e => e.Facturas).HasForeignKey(f => f.EmpresaId).OnDelete(DeleteBehavior.Restrict)` y el check:
   ```csharp
   modelBuilder.Entity<Factura>().ToTable(t => t.HasCheckConstraint("CK_Facturas_Mes", "[Mes] BETWEEN 1 AND 12"));
   ```
7. `appsettings.json`: agrega
   ```json
   "ConnectionStrings": {
     "SageApi": "Server=.\\SQLEXPRESS;Database=SAGE_API;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
   }
   ```
   En Mac/Docker **no** cambies ese archivo: usa
   `dotnet user-secrets set "ConnectionStrings:SageApi" "Server=localhost,1433;Database=SAGE_API;User Id=sa;Password=<TU_PASSWORD>;TrustServerCertificate=True"` (la persona escribe su contraseña; tú no la ves ni la guardas).
8. `Properties/launchSettings.json`: deja **solo** el perfil `http` con `"applicationUrl": "http://localhost:5080"`, `"launchBrowser": true` y `"launchUrl": "swagger"`.
9. `Program.cs` queda exactamente así:
   ```csharp
   using System.Reflection;
   using Microsoft.EntityFrameworkCore;
   using Microsoft.OpenApi; // Swashbuckle 10+. Si no compila, usa Microsoft.OpenApi.Models
   using SAGE.Api.Data;

   var builder = WebApplication.CreateBuilder(args);

   builder.Services.AddDbContext<SageApiDbContext>(o =>
       o.UseSqlServer(builder.Configuration.GetConnectionString("SageApi")));
   builder.Services.AddControllers();
   builder.Services.AddProblemDetails();
   builder.Services.AddEndpointsApiExplorer();
   builder.Services.AddSwaggerGen(c =>
   {
       c.SwaggerDoc("v1", new OpenApiInfo
       {
           Title = "SAGE API",
           Version = "v1",
           Description = "Empresas y facturas de energía del Sistema de Análisis y Gestión Energética (SAGE)."
       });
       c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
   });

   var app = builder.Build();

   app.UseExceptionHandler();
   if (app.Environment.IsDevelopment())
   {
       app.UseSwagger();
       app.UseSwaggerUI();
   }
   app.MapControllers();
   app.Run();
   ```
10. **Comprobar:** `dotnet build` sin errores. Commit `API-M1`.

### - [x] API-M2 · Datos de prueba, migración y README base

1. `Data/DatosSemilla.cs`: clase estática con un método `Cargar(ModelBuilder modelBuilder)` que llama a `HasData` con **exactamente** estos datos, y llámalo al final de `OnModelCreating`. Fecha fija para todos: `new DateTime(2026, 7, 1, 12, 0, 0, DateTimeKind.Utc)` (nunca `DateTime.UtcNow` en la semilla).

   | Id | Nombre | Nit | Direccion | Activa |
   |---|---|---|---|---|
   | 1 | Energía Andina S.R.L. | 1023456027 | Av. Cristo Redentor 2450, Santa Cruz | true |
   | 2 | Hotel Las Palmas | 2034567018 | Calle Libertad 120, Santa Cruz | true |
   | 3 | Panadería El Trigal | 3045678019 | Av. Beni 890, Santa Cruz | true |

   | Id | EmpresaId | Anio | Mes | ConsumoKwh | MontoBs |
   |---|---|---|---|---|---|
   | 1 | 1 | 2026 | 1 | 2150.000 | 1935.00 |
   | 2 | 1 | 2026 | 2 | 2080.500 | 1872.45 |
   | 3 | 1 | 2026 | 3 | 2210.000 | 1989.00 |
   | 4 | 1 | 2026 | 4 | 1985.250 | 1786.73 |
   | 5 | 1 | 2026 | 5 | 1890.000 | 1701.00 |
   | 6 | 1 | 2026 | 6 | 1820.750 | 1638.68 |
   | 7 | 2 | 2026 | 1 | 3420.000 | 3078.00 |
   | 8 | 2 | 2026 | 2 | 3315.500 | 2983.95 |
   | 9 | 2 | 2026 | 3 | 3502.000 | 3151.80 |

   La empresa 3 no tiene facturas a propósito (sirve para probar un DELETE 204).
2. Migración y base (desde la raíz):
   ```
   dotnet ef migrations add Inicial --project src/SAGE.Api --output-dir Migrations
   dotnet ef database update --project src/SAGE.Api
   ```
   Abre la migración y confirma que tiene las dos tablas, la FK con `ReferentialAction.Restrict`, los dos índices únicos, el check `CK_Facturas_Mes` y los `InsertData` de las 3 empresas y 9 facturas.
3. `README.md` base con estas secciones vacías o con lo que ya sepas: título `SAGE API`, versión de .NET, tabla **Integrantes** (`Nombre | Usuario de GitHub | Aporte`) con tu fila completa (Mateo Montero Gonzales), enlace al repositorio, **Configurar la conexión**, **Aplicar las migraciones**, **Datos de prueba**, **Ejecutar y abrir Swagger**. Harold lo completa en M8.
4. **Comprobar:** `dotnet run --project src/SAGE.Api` arranca y `http://localhost:5080/swagger` abre (sin endpoints todavía). `dotnet ef migrations list --project src/SAGE.Api` muestra `Inicial` aplicada. Commit `API-M2`.

---

## 6. Matías — DTO y controlador de empresas

### - [ ] API-M3 · DTO de entrada y salida

Archivos `Dtos/EmpresaDtos.cs` y `Dtos/FacturaDtos.cs`, namespace `SAGE.Api.Dtos`. Las propiedades de entrada son **anulables** con `[Required]`: así, si el cliente omite un campo, la respuesta es 400 y no un 0 silencioso.

```csharp
/// <summary>Datos para crear o actualizar una empresa.</summary>
public class EmpresaEntradaDto
{
    /// <example>Clínica San Lucas</example>
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 150 caracteres.")]
    public string? Nombre { get; set; }

    /// <example>4455667011</example>
    [Required(ErrorMessage = "El NIT es obligatorio.")]
    [RegularExpression(@"^\d{5,20}$", ErrorMessage = "El NIT debe tener de 5 a 20 dígitos, sin letras ni espacios.")]
    public string? Nit { get; set; }

    /// <example>Av. Banzer 450, Santa Cruz</example>
    [StringLength(250, ErrorMessage = "La dirección no puede superar 250 caracteres.")]
    public string? Direccion { get; set; }

    public bool Activa { get; set; } = true;
}

/// <summary>Empresa devuelta por la API.</summary>
public record EmpresaDto(int Id, string Nombre, string Nit, string? Direccion, bool Activa, DateTime FechaRegistro, int CantidadFacturas);
```

```csharp
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
public record FacturaDto(int Id, int EmpresaId, string EmpresaNombre, int Anio, int Mes, string Periodo,
                         decimal ConsumoKwh, decimal MontoBs, decimal TarifaPromedioBsKwh, DateTime FechaRegistro)
{
    private static readonly string[] Meses =
        ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"];

    /// <summary>Convierte una factura con su <c>Empresa</c> cargada (Include).</summary>
    public static FacturaDto Desde(Factura f) => new(f.Id, f.EmpresaId, f.Empresa.Nombre, f.Anio, f.Mes,
        $"{Meses[f.Mes - 1]} {f.Anio}", f.ConsumoKwh, f.MontoBs, Math.Round(f.MontoBs / f.ConsumoKwh, 4), f.FechaRegistro);
}
```

**Comprobar:** `dotnet build`. Commit `API-M3`.

### - [ ] API-M4 · `EmpresasController`

`Controllers/EmpresasController.cs`: `[ApiController]`, `[Route("api/empresas")]`, `[Produces("application/json")]`, constructor que recibe `SageApiDbContext`. Cada acción es `async Task<ActionResult<...>>` (o `Task<IActionResult>` en DELETE), lleva `/// <summary>`, un `/// <response code="...">` por cada código y un `[ProducesResponseType]` por cada código (`typeof(ValidationProblemDetails)` para 400, `typeof(ProblemDetails)` para 404 y 409).

| Acción | Comportamiento |
|---|---|
| `Listar()` `GET` | `AsNoTracking`, orden por `Nombre`, proyección `Select(e => new EmpresaDto(..., e.Facturas.Count))` |
| `ObtenerPorId(int id)` `GET {id:int}` | 404 `title: "Empresa no encontrada"`, `detail: "No existe la empresa con id {id}."` |
| `ListarFacturas(int id)` `GET {id:int}/facturas` | 404 igual al anterior si la empresa no existe; si existe, sus facturas con `Include(f => f.Empresa)`, orden `Anio`, `Mes`, mapeadas con `FacturaDto.Desde` (puede ser lista vacía) |
| `Crear(EmpresaEntradaDto dto)` `POST` | `Nombre` y `Nit` con `Trim()`. Si otro registro tiene ese NIT: 409 `title: "NIT repetido"`, `detail: "Ya existe una empresa con el NIT {nit}."`. Si no: guarda la entidad `e` y `return CreatedAtAction(nameof(ObtenerPorId), new { id = e.Id }, <EmpresaDto de e con CantidadFacturas 0>)` |
| `Actualizar(int id, EmpresaEntradaDto dto)` `PUT {id:int}` | 404 si no existe; 409 si el NIT lo tiene **otra** empresa; si no, actualiza `Nombre`, `Nit`, `Direccion`, `Activa` y devuelve 200 con el `EmpresaDto` |
| `Eliminar(int id)` `DELETE {id:int}` | 404 si no existe; si tiene facturas: 409 `title: "Empresa con facturas"`, `detail: "La empresa {id} tiene {n} facturas. Elimínalas antes de borrar la empresa."`; si no, borra y 204 |

Las validaciones de `[Required]`, `[StringLength]` y `[RegularExpression]` las aplica `[ApiController]` solo (400 automático): no las repitas a mano.

**Comprobar** con la app en marcha (en el `SAGE.Api.http` o en Swagger): E01 a E10 de la sección 8 devuelven su código. Agrega tu fila en la tabla Integrantes del README. Commit `API-M4`.

---

## 7. Ethan — controlador de facturas y archivo de pruebas

### - [ ] API-M5 · `FacturasController` y reglas del dominio

1. En `Models/Factura.cs` agrega:
   ```csharp
   /// <summary>Regla del dominio: no se registra la factura de un mes que todavía no llegó.</summary>
   public static bool EsPeriodoFuturo(int anio, int mes)
   {
       var hoy = DateTime.UtcNow.AddHours(-4); // Bolivia es UTC-4 todo el año
       return anio > hoy.Year || (anio == hoy.Year && mes > hoy.Month);
   }
   ```
2. `Controllers/FacturasController.cs` con los mismos atributos y reglas de documentación que M4, ruta `api/facturas`.

| Acción | Comportamiento |
|---|---|
| `Listar(int? empresaId)` `GET` | `[FromQuery]`; `Include(f => f.Empresa)`, `AsNoTracking`, filtro si viene `empresaId`, orden `EmpresaId`, `Anio`, `Mes`, `FacturaDto.Desde` |
| `ObtenerPorId(int id)` `GET {id:int}` | 404 `title: "Factura no encontrada"`, `detail: "No existe la factura con id {id}."` |
| `Crear(FacturaEntradaDto dto)` `POST` | Validaciones en este orden (cada una corta): **(a)** empresa inexistente → `ModelState.AddModelError("EmpresaId", "La empresa {id} no existe.")` y `ValidationProblem(ModelState)` (400); **(b)** empresa inactiva → igual con `"La empresa {id} está inactiva y no admite facturas."`; **(c)** `Factura.EsPeriodoFuturo` → clave `"Mes"`, `"El periodo {mes}/{anio} es futuro: solo se registran meses ya facturados."`; **(d)** ya existe esa empresa + año + mes → 409 `title: "Periodo repetido"`, `detail: "La empresa {id} ya tiene la factura de {mes}/{anio}."`. Guarda, recarga con la empresa y `CreatedAtAction(nameof(ObtenerPorId), new { id }, FacturaDto)` |
| `Actualizar(int id, FacturaEntradaDto dto)` `PUT {id:int}` | 404 si no existe; luego (a)–(d) igual que POST, y en (d) se excluye la propia factura (`f.Id != id`); actualiza los 5 campos y devuelve 200 con el `FacturaDto` |
| `Eliminar(int id)` `DELETE {id:int}` | 404 si no existe; si no, borra y 204 |

3. En `Crear` y `Actualizar`, envuelve `SaveChangesAsync` en `try/catch (DbUpdateException)` y responde el mismo 409 de (d): el índice único es la última defensa si dos personas registran el mismo mes a la vez.

### - [ ] API-M6 · `SAGE.Api.http` con todas las pruebas

Reemplaza `src/SAGE.Api/SAGE.Api.http` por las 23 peticiones de la sección 8, en ese orden, cada una precedida por `### E<nn> · <operación> → <código esperado>` y con `@base = http://localhost:5080` arriba. Usa exactamente los cuerpos de la sección 8.

**Comprobar:** borra y recrea la base (`dotnet ef database drop --force --project src/SAGE.Api` y luego `database update`), arranca la app y ejecuta E01 a E23 en orden (con la extensión REST Client, el `.http` de Visual Studio/Rider o `curl`). Todas deben dar el código esperado; E20 pide reiniciar la app. Anota en la Bitácora cualquier diferencia. Agrega tu fila en Integrantes del README. Commit por misión (`API-M5`, `API-M6`).

---

## 8. Pruebas E01–E23 (las ejecuta Ethan en M6 y las captura Harold en M7)

Requieren una base **recién creada** (ids de la semilla). La empresa nueva será el id 4 y la factura nueva el id 10.

| Prueba | Operación | Cuerpo / detalle | Esperado |
|---|---|---|---|
| E01 | `GET /api/empresas` | — | 200, 3 empresas |
| E02 | `GET /api/empresas/1` | — | 200, `cantidadFacturas: 6` |
| E03 | `GET /api/empresas/999` | — | **404** |
| E04 | `POST /api/empresas` | `{"nombre":"Clínica San Lucas","nit":"4455667011","direccion":"Av. Banzer 450, Santa Cruz","activa":true}` | **201**, `Location: .../api/empresas/4` |
| E05 | `POST /api/empresas` | `{"nombre":"","nit":"ABC-12"}` | **400**, errores en `Nombre` y `Nit` |
| E06 | `POST /api/empresas` | `{"nombre":"Copia","nit":"1023456027"}` | **409** NIT repetido |
| E07 | `PUT /api/empresas/4` | `{"nombre":"Clínica San Lucas","nit":"4455667011","direccion":"Av. Banzer 455, 3er anillo, Santa Cruz","activa":true}` | 200, dirección nueva |
| E08 | `PUT /api/empresas/999` | mismo cuerpo que E07 | **404** |
| E09 | `GET /api/empresas/1/facturas` | recurso relacionado | 200, 6 facturas Enero–Junio 2026 |
| E10 | `GET /api/empresas/999/facturas` | — | **404** |
| E11 | `GET /api/facturas?empresaId=2` | parámetro de consulta | 200, 3 facturas |
| E12 | `GET /api/facturas/1` | — | 200, `periodo: "Enero 2026"` |
| E13 | `POST /api/facturas` | `{"empresaId":4,"anio":2026,"mes":10,"consumoKwh":1750.5,"montoBs":1575.45}` | **201**, `Location: .../api/facturas/10` |
| E14 | `POST /api/facturas` | `{"empresaId":999,"anio":2026,"mes":10,"consumoKwh":100,"montoBs":90}` | **400** referencia inválida (`EmpresaId`) |
| E15 | `POST /api/facturas` | `{"empresaId":1,"anio":2026,"mes":13}` | **400** mes fuera de rango, falta consumo y monto |
| E16 | `POST /api/facturas` | `{"empresaId":1,"anio":2030,"mes":1,"consumoKwh":100,"montoBs":90}` | **400** regla del dominio (periodo futuro) |
| E17 | `POST /api/facturas` | `{"empresaId":1,"anio":2026,"mes":1,"consumoKwh":100,"montoBs":90}` | **409** periodo repetido |
| E18 | `PUT /api/facturas/10` | `{"empresaId":4,"anio":2026,"mes":10,"consumoKwh":1802.25,"montoBs":1622.03}` | 200, valores nuevos |
| E19 | `DELETE /api/empresas/1` | — | **409** tiene facturas |
| E20 | **Reiniciar la app** y `GET /api/empresas/4` + `GET /api/facturas/10` | persistencia | 200, con la dirección de E07 y el consumo de E18 |
| E21 | `DELETE /api/facturas/10` | — | **204** |
| E22 | `DELETE /api/facturas/10` | otra vez | **404** |
| E23 | `DELETE /api/empresas/4` | ya sin facturas | **204** |

---

## 9. Harold — documentación de Swagger, evidencias, README y entrega

### - [ ] API-M7 · Swagger completo y PDF de evidencias

1. Revisa en Swagger UI que **cada** operación muestre: ruta, parámetros (`id`, `empresaId`), esquema de entrada con ejemplos, y todas sus respuestas con esquema (`EmpresaDto`, `FacturaDto`, `ValidationProblemDetails`, `ProblemDetails`). Corrige lo que falte en los controladores (comentarios XML o `[ProducesResponseType]`).
2. Base recién creada (drop + update), app en marcha. En Swagger UI ejecuta E01–E23 con **Try it out** y toma una captura de cada una donde se vean la operación, el cuerpo enviado y la respuesta (código y cuerpo; en los 201 también el encabezado `location`). Si el agente puede manejar un navegador, que lo haga él; si no, la persona toma las capturas siguiendo la tabla. Nombres: `docs/evidencias/E01_GET_empresas_200.png`, `E03_GET_empresas_id_404.png`, etc. Más una captura `E00_swagger_endpoints.png` con la lista de endpoints y otra `E00_swagger_esquemas.png` con los esquemas.
3. `docs/evidencias/evidencias.html`: portada (SAGE API, integrantes, fecha, enlace al repo), y una sección por prueba con título `E<nn> · <MÉTODO> <ruta> → <código>`, qué demuestra (éxito, 400, 404, 409 o persistencia) y su captura. Para E20 explica los pasos: se detuvo la app (Ctrl+C), se volvió a ejecutar y el GET devolvió los datos de E07 y E18.
4. PDF. En Mac: abre `evidencias.html` en Chrome o Safari → Imprimir → Guardar como PDF en `docs/evidencias/Evidencias_SAGE_API.pdf` (sin encabezados ni pies). En Windows, con Edge:
   ```
   & "C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe" --headless --disable-gpu --no-pdf-header-footer "--print-to-pdf=docs\evidencias\Evidencias_SAGE_API.pdf" "file:///<ruta absoluta>/docs/evidencias/evidencias.html"
   ```
   Abre el PDF y comprueba que se leen todas las capturas. Commit `API-M7`.

### - [ ] API-M8 · README final, ZIP y revisión

1. README completo: nombre del proyecto, versión de .NET (10) y de los paquetes, tabla Integrantes con los 4 (`Mateo Montero Gonzales`, `Harold Henry Chuve Nava`, `Matias Nahim Justiniano Suarez`, `Ethan Matias Cordova Rodriguez`) con su usuario de GitHub y su aporte (misiones), enlace al repo, y los pasos **reproducibles**: requisitos, configurar la conexión (cadena de ejemplo de `appsettings.json` y la variante user-secrets con `<TU_PASSWORD>`, nunca una contraseña real), aplicar migraciones (`dotnet ef database update --project src/SAGE.Api`), datos de prueba (vienen en la migración; tabla resumida), ejecutar (`dotnet run --project src/SAGE.Api`) y abrir `http://localhost:5080/swagger`, tabla de endpoints (sección 4), pruebas (`SAGE.Api.http` y el PDF).
2. `scripts/empaquetar.ps1` (en Mac sin PowerShell, ejecuta desde la raíz `git archive --format=zip --prefix=SAGE_API/ -o SAGE_API.zip HEAD`, que hace lo mismo):
   ```powershell
   # Genera SAGE_API.zip con lo que está en el último commit: sin bin, obj, .vs ni esta guía (.gitattributes).
   $raiz = Split-Path $PSScriptRoot -Parent
   git -C $raiz archive --format=zip --prefix=SAGE_API/ -o (Join-Path $raiz 'SAGE_API.zip') HEAD
   ```
3. Commit y push de todo **antes** de empaquetar (el ZIP sale del último commit). Ejecuta el script y comprueba con `tar -tf SAGE_API.zip` que tiene `README.md`, `docs/evidencias/Evidencias_SAGE_API.pdf`, `src/SAGE.Api/Migrations/` y **no** tiene `bin/`, `obj/`, `.vs/` ni `GUIA_TRABAJO_API.md`.
4. Prueba de reproducibilidad: descomprime el ZIP en una carpeta temporal, cambia el nombre de la base en la cadena a `SAGE_API_PRUEBA`, `dotnet ef database update`, `dotnet run` y abre Swagger. Borra esa base al terminar.
5. Revisa `git shortlog -sne` y confirma que aparecen los 4 integrantes con commits. Commit `API-M8`.

---

## 10. Bitácora

Cada agente agrega **una entrada al terminar** (y otra si deja un BLOQUEO). No borres entradas de otros.

```text
### <fecha y hora> — <Nombre> — <misiones>
- Hecho: ...
- Desvíos de la guía y motivo: ... (o "ninguno")
- Pruebas que no dieron el código esperado: ... (o "ninguna")
- Pendientes o BLOQUEOS: ... (o "ninguno")
- Último commit: <hash corto>
```

<!-- Las entradas van debajo de esta línea -->

### 2026-11-19 17:36 (UTC-4) — Mateo — API-M1 y API-M2

- Hecho: solución, proyecto, modelo, DbContext, Swagger, semilla, migración `Inicial` y README base. SQL Server Express confirmó 3 empresas y 9 facturas. Swagger respondió HTTP 200 y la aplicación se detuvo después de la prueba. Ambas compilaciones terminaron sin errores, con avisos NU1902 y NU1903 de NuGet sobre `System.Security.Cryptography.Pkcs` 9.0.11.
- Desvíos de la guía y motivo: el `git pull` inicial no encontró `origin/main` porque el remoto estaba vacío. Por instrucción de Mateo se omitió `dotnet user-secrets init` y se usó la conexión de Windows de `appsettings.json`. La restauración guardó EF Core Tools en la caché global de NuGet.
- Pruebas que no dieron el código esperado: ninguna.
- Pendientes o BLOQUEOS: ninguno.
- Último commit: 8cdd800
- Ajuste: user-secrets init y Pkcs parcheado (ver commit siguiente al de API-M2)
