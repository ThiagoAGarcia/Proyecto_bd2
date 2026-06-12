using api.Methods;
using MySqlConnector;
using api.DTOs;

namespace api.Endpoints;

public static class EquipoEndpoints
{
    public static void MapEquipoEndpoints(this WebApplication app)
    {
        app.MapPost("/equipo", async (EquipoRequest request, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            var Nombre = Normalizar.NormalizarMethod(request.Nombre);

            await using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO `Equipo` (`Nombre`, `Bandera`)
                VALUES (@nombre, @bandera);
                """;

            command.Parameters.AddWithValue("@nombre", Nombre);
            command.Parameters.AddWithValue("@bandera", request.Bandera);

            await command.ExecuteNonQueryAsync();

            return Results.Created($"/equipo", new
            {
                Nombre = Nombre,
                Bandera = request.Bandera
            });
        }).RequireAuthorization("SoloAdministrador");

        app.MapDelete("/equipo/{nombre}", async (string nombre, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                DELETE FROM `Equipo`
                WHERE `Nombre` = @nombre;
                """;

            command.Parameters.AddWithValue("@nombre", nombre);

            var rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected == 0)
            {
                return Results.NotFound(new
                {
                    message = "Equipo no encontrado"
                });
            }

            return Results.NoContent();
        }).RequireAuthorization("SoloAdministrador");

        app.MapPut("/equipo/{nombre}", async (string nombre, EquipoUpdateRequest request, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                UPDATE `Equipo`
                SET `Bandera` = @bandera
                WHERE `Nombre` = @nombre;
                """;

            command.Parameters.AddWithValue("@nombre", nombre);
            command.Parameters.AddWithValue("@bandera", request.Bandera);

            var rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected == 0)
            {
                return Results.NotFound(new
                {
                    message = "Equipo no encontrado"
                });
            }

            return Results.NoContent();
        }).RequireAuthorization("SoloAdministrador");

        app.MapGet("/equipo/{nombre}", async (string nombre, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT `Nombre`, `Bandera`
                FROM `Equipo`
                WHERE `Nombre` = @nombre
                LIMIT 1;
                """;

            command.Parameters.AddWithValue("@nombre", nombre);

            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return Results.NotFound(new
                {
                    message = "Equipo no encontrado"
                });
            }

            return Results.Ok(new
            {
                Nombre = reader.GetString("Nombre"),
                Bandera = reader.GetString("Bandera")
            });
        }).RequireAuthorization();

        app.MapGet("/allEquipo", async (IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT `Nombre`, `Bandera`
                FROM `Equipo`;
                """;

            await using var reader = await command.ExecuteReaderAsync();

            var equipos = new List<object>();

            while (await reader.ReadAsync())
            {
                equipos.Add(new
                {
                    Nombre = reader.GetString("Nombre"),
                    Bandera = reader.GetString("Bandera")
                });
            }

            return Results.Ok(equipos);
        }).RequireAuthorization();
    }
}