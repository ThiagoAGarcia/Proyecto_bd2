using Microsoft.EntityFrameworkCore;

using api.Data;
using api.Endpoints;
var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

app.MapGet("/", () => "API funcionando");

app.MapGet("/db-check", async (AppDbContext db) =>
{
    try
    {
        var canConnect = await db.Database.CanConnectAsync();

        return canConnect
            ? Results.Ok("Conexion a BD correcta")
            : Results.Problem("No se pudo conectar a la BD");
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.ToString(),
            title: "Error conectando a la BD"
        );
    }
});


// Perfil endpoints
app.MapPerfilEndpoints();

app.Run();