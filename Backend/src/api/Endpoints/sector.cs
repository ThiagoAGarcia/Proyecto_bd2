using api.Methods;
using MySqlConnector;
using api.DTOs;

namespace api.Endpoints;

public static class SectorEndpoints
{
    public static void MapSectorEndpoints(this WebApplication app)
    {
        app.MapPost("/nuevoSector", async (IConfiguration config, SectorRequest request) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO Sector VALUES
                (@identificadorSector, @identificadorEstadio, @nombre, @capMax, @tarifaExtra);
                """;

            command.Parameters.AddWithValue("@identificadorSector", request.Identificador);
            command.Parameters.AddWithValue("@identificadorEstadio", request.IdentificadorEstadio);
            command.Parameters.AddWithValue("@nombre", request.Nombre);
            command.Parameters.AddWithValue("@capMax", request.CapMax);
            command.Parameters.AddWithValue("@tarifaExtra", request.TarifaExtra);

            try
            {
                await command.ExecuteNonQueryAsync();
            }
            catch (MySqlException ex)
            {
                return Results.Conflict(new{
                    success = false
                });
            }

            return Results.Ok(new
            {
                success = true,
                message = "El sector ha sido agregado correctamente"
            });
        }).RequireAuthorization("SoloAdministrador");

        app.MapGet("/allSectores/{estadio}", async (IConfiguration config, int estadio) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT `identificador`, `identificadorEstadio`, `nombre`, `capMax`, `tarifaExtra`
                FROM `Sector`
                WHERE identificadorEstadio = @identificadorEstadio;
                """;

            command.Parameters.AddWithValue("@identificadorEstadio", estadio);

            await using var reader = await command.ExecuteReaderAsync();

            var sectores = new List<object>();

            while (await reader.ReadAsync())
            {
                sectores.Add(new
                {
                    Identificador = reader.GetInt32("identificador"),
                    IdentificadorEstadio = reader.GetInt32("identificadorEstadio"),
                    Nombre = reader.GetString("nombre"),
                    CapMax = reader.GetInt32("capMax"),
                    TarifaExtra = reader.GetInt32("tarifaExtra")
                });
            }

            return Results.Ok(sectores);
        }).RequireAuthorization("SoloAdministrador");

        app.MapPut("/sector/editar", async (IConfiguration config, SectorUpdateRequest request) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                UPDATE `Sector`
                SET 
                    `nombre` = @nombre,
                    `capMax` = @capMax,
                    `tarifaExtra` = @tarifaExtra
                WHERE 
                    `identificador` = @identificador AND
                    `identificadorEstadio` = @identificadorEstadio;
                """;

            command.Parameters.AddWithValue("@identificador", request.Identificador);
            command.Parameters.AddWithValue("@identificadorEstadio", request.IdentificadorEstadio);
            command.Parameters.AddWithValue("@nombre", request.Nombre);
            command.Parameters.AddWithValue("@capMax", request.CapMax);
            command.Parameters.AddWithValue("@tarifaExtra", request.TarifaExtra);

            var rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected == 0)
            {
                return Results.NotFound(new
                {
                    message = "Sector no encontrado"
                });
            }

            return Results.NoContent();
        }).RequireAuthorization("SoloAdministrador");

        app.MapDelete("/sector/borrar/{identificadorEstadio}/{identificadorSector}", async (int identificadorSector, int identificadorEstadio, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                DELETE FROM `Sector`
                WHERE `identificador` = @identificadorSector AND `identificadorEstadio` = @identificadorEstadio;
                """;

            command.Parameters.AddWithValue("@identificadorSector", identificadorSector);
            command.Parameters.AddWithValue("@identificadorEstadio", identificadorEstadio);

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
    }
}