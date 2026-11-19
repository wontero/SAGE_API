# SAGE API

Web API de empresas y facturas de energía del Sistema de Análisis y Gestión Energética (SAGE).

- .NET 10 (`net10.0`)
- EF Core 10.0.11
- Swashbuckle.AspNetCore 11.0.1
- [Repositorio en GitHub](https://github.com/wontero/SAGE_API)

## Integrantes

| Nombre | Usuario de GitHub | Aporte |
|---|---|---|
| Mateo Montero Gonzales | `wontero` | API-M1 y API-M2. Solución, modelo, DbContext, Swagger, semilla, migración `Inicial` y README base. |

## Configurar la conexión

`src/SAGE.Api/appsettings.json` usa `ConnectionStrings:SageApi` para conectarse a `SAGE_API` en SQL Server Express (`.\SQLEXPRESS`) con autenticación de Windows.

## Aplicar las migraciones

Desde la raíz del repositorio, con `dotnet-ef` instalado:

```powershell
dotnet ef database update --project src/SAGE.Api
```

## Datos de prueba

La migración `Inicial` carga 3 empresas y 9 facturas de 2026. Energía Andina S.R.L. tiene seis facturas, Hotel Las Palmas tiene tres y Panadería El Trigal queda disponible para probar el borrado de una empresa sin facturas.

## Ejecutar y abrir Swagger

```powershell
dotnet run --project src/SAGE.Api
```

Abre [Swagger UI](http://localhost:5080/swagger).
