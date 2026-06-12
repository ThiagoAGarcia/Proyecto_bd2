using api.Methods;
using MySqlConnector;
using api.DTOs;

namespace api.Endpoints;

public static class PaisEndpoints
{
    public static void MapPaisEndpoints(this WebApplication app)
    {
        app.MapPost("/pais", async (PaisRequest request, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            var Nombre = Normalizar.NormalizarMethod(request.Nombre);

            await using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO `pais` (`Nombre`)
                VALUES (@nombre);
                """;

            command.Parameters.AddWithValue("@nombre", Nombre);

            await command.ExecuteNonQueryAsync();

            return Results.Created($"/pais", new
            {
                Nombre = request.Nombre
            });
        }).RequireAuthorization("SoloAdministrador");

        app.MapGet("/pais/{nombre}", async (string nombre, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT `Nombre`
                FROM `pais`
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
                Nombre = reader.GetString("Nombre")
            });
        }).RequireAuthorization("AdminOFuncionario");

        app.MapDelete("/pais/{nombre}", async (string nombre, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                DELETE FROM `pais`
                WHERE `Nombre` = @nombre;
                """;

            command.Parameters.AddWithValue("@nombre", nombre);

            var affectedRows = await command.ExecuteNonQueryAsync();

            if (affectedRows == 0)
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        }).RequireAuthorization("SoloAdministrador");

    }
}