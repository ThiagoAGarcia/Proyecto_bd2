using api.Data;
using api.Endpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

string key = "/o/VObMvm>.fa@80p:9P.b?/Ox.Mxk7mP+|£'3&$,+||cook";

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();
builder.Services.AddAuthorization();


var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();





app.MapGet("/", () => "API funcionando").RequireAuthorization();

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