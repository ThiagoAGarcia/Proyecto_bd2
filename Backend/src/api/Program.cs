using api.Endpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new Exception("No esta configurada la connection string DefaultConnection");
}

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
    throw new Exception("No esta configurada la clave JWT");
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

app.MapGet("/db-check", async (IConfiguration config) =>
{
    try
    {
        await using var connection = new MySqlConnection(config.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT 1;";

        var result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result) == 1
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
app.MapUsuarioEndpoints();
app.MapAdministradorEndpoints();
app.MapFuncionarioEndpoints();
app.MapVerificarMailEndpoints();
app.MapEstadioEndpoints();
app.MapPaisEndpoints();
app.MapTelefonoEndpoints();

app.Run();
