using api.Data;
using api.Endpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// CORS para React
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new Exception("No está configurada la clave JWT");
}

var byteKey = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(byteKey),

            ValidateIssuer = false,
            ValidateAudience = false,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["access_token"];
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SoloAdministrador", policy =>
        policy.RequireRole("Administrador"));

    options.AddPolicy("SoloFuncionario", policy =>
        policy.RequireRole("Funcionario"));

    options.AddPolicy("SoloUsuario", policy =>
        policy.RequireRole("Usuario"));

    options.AddPolicy("AdminOFuncionario", policy =>
        policy.RequireRole("Administrador", "Funcionario"));
});

var app = builder.Build();

app.UseCors("ReactPolicy");

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

app.MapPerfilEndpoints();
app.MapLoginEndpoints();

app.Run();