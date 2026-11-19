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
