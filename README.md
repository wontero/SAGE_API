# SAGE API

Web API REST de **empresas** y **facturas de energía** del Sistema de Análisis y Gestión Energética (SAGE).

Proyecto independiente de la app Blazor SAGE: repositorio propio, base `SAGE_API` y migraciones propias.

- [Repositorio en GitHub](https://github.com/wontero/SAGE_API)
- .NET 10 (`net10.0`)
- EF Core **10.0.11**
- Swashbuckle.AspNetCore **11.0.1**
- URL local: `http://localhost:5080` · Swagger: `http://localhost:5080/swagger`

## Integrantes

| Nombre | Usuario de GitHub | Aporte |
|---|---|---|
| Mateo Montero Gonzales | `wontero` | API-M1 y API-M2 (solución, modelo, DbContext, Swagger, semilla, migración `Inicial`, README base). API-M5 y API-M6 (FacturasController y pruebas E01–E23). |
| Matias Nahim Justiniano Suarez | `lowgaara909-sudo` | API-M3 y API-M4 (DTOs y EmpresasController). |
| Ethan Matias Cordova Rodriguez | `kawerlev-a11y` | Misiones API-M5 y API-M6 asignadas en la guía; implementadas por Mateo. |
| Harold Henry Chuve Nava | `hnava08` | API-M7 y API-M8 (Swagger, evidencias E01–E23, PDF, README final, empaquetado ZIP). |

## Requisitos

- SDK de **.NET 10** (`dotnet --list-sdks` debe mostrar una `10.0.x`)
- SQL Server (Windows: Express `.\SQLEXPRESS`; Mac: Azure SQL Edge / SQL Server en Docker)
- Herramienta EF Core: `dotnet tool install --global dotnet-ef`

## Configurar la conexión

`src/SAGE.Api/appsettings.json` usa:

```json
"ConnectionStrings": {
  "SageApi": "Server=.\\SQLEXPRESS;Database=SAGE_API;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

En Mac / Docker **no** cambies ese archivo. Usa user-secrets (reemplaza `<TU_PASSWORD>` por tu contraseña SA):

```bash
dotnet user-secrets set "ConnectionStrings:SageApi" "Server=localhost,1433;Database=SAGE_API;User Id=sa;Password=<TU_PASSWORD>;TrustServerCertificate=True;MultipleActiveResultSets=true" --project src/SAGE.Api
```

## Aplicar las migraciones

Desde la raíz del repositorio:

```bash
dotnet ef database update --project src/SAGE.Api
```

## Datos de prueba

La migración `Inicial` carga 3 empresas y 9 facturas de 2026:

| Empresa | NIT | Facturas |
|---|---|---|
| Energía Andina S.R.L. | 1023456027 | 6 (enero–junio 2026) |
| Hotel Las Palmas | 2034567018 | 3 (enero–marzo 2026) |
| Panadería El Trigal | 3045678019 | 0 (sirve para DELETE 204) |

## Ejecutar y abrir Swagger

```bash
dotnet run --project src/SAGE.Api
```

Abre [http://localhost:5080/swagger](http://localhost:5080/swagger).

## Endpoints

| Método y ruta | Éxito | Errores |
|---|---|---|
| `GET /api/empresas` | 200 lista de `EmpresaDto` | — |
| `GET /api/empresas/{id}` | 200 `EmpresaDto` | 404 |
| `GET /api/empresas/{id}/facturas` | 200 lista de `FacturaDto` | 404 |
| `POST /api/empresas` | 201 + `Location` | 400, 409 NIT repetido |
| `PUT /api/empresas/{id}` | 200 | 400, 404, 409 |
| `DELETE /api/empresas/{id}` | 204 | 404, 409 si tiene facturas |
| `GET /api/facturas?empresaId=` | 200 | — |
| `GET /api/facturas/{id}` | 200 | 404 |
| `POST /api/facturas` | 201 + `Location` | 400, 409 periodo repetido |
| `PUT /api/facturas/{id}` | 200 | 400, 404, 409 |
| `DELETE /api/facturas/{id}` | 204 | 404 |

## Pruebas

- Archivo HTTP: `src/SAGE.Api/SAGE.Api.http` (E01–E23)
- Evidencias: `docs/evidencias/evidencias.html` y `docs/evidencias/Evidencias_SAGE_API.pdf`

Para repetir las pruebas con base limpia:

```bash
dotnet ef database drop --force --project src/SAGE.Api
dotnet ef database update --project src/SAGE.Api
dotnet run --project src/SAGE.Api
```

## Empaquetar (ZIP de entrega)

En Windows (PowerShell):

```powershell
./scripts/empaquetar.ps1
```

En Mac / Linux (equivalente):

```bash
git archive --format=zip --prefix=SAGE_API/ -o SAGE_API.zip HEAD
```

El ZIP sale del último commit (sin `bin/`, `obj/`, `.vs/` ni `GUIA_TRABAJO_API.md`).
