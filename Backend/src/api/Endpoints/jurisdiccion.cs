using api.Methods;
using MySqlConnector;
using api.DTOs;

namespace api.Endpoints;

public static class JurisdiccionEndpoints
{
    public static void MapJurisdiccionEndpoints(this WebApplication app)
    {
        app.MapPost("/jurisdiccion", async (JurisdiccionRequest request, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO `jurisdiccion` (`Nombre`, `Continente`)
                VALUES (@nombre, @continente);
                """;

            command.Parameters.AddWithValue("@nombre", request.Nombre);
            command.Parameters.AddWithValue("@continente", request.Continente);

            await command.ExecuteNonQueryAsync();

            return Results.Created($"/jurisdiccion", new
            {
                Nombre = request.Nombre,
                Continente = request.Continente
            });
        }).RequireAuthorization("SoloAdministrador");

        app.MapGet("/jurisdiccion/{nombre}", async (string nombre, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT `Nombre`, `Continente`
                FROM `jurisdiccion`
                WHERE `Nombre` = @nombre
                LIMIT 1;
                """;

            command.Parameters.AddWithValue("@nombre", nombre);

            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return Results.NotFound();
            }

            return Results.Ok(new
            {
                Nombre = reader.GetString("Nombre"),
                Continente = reader.GetString("Continente")
            });
        });

    }
}