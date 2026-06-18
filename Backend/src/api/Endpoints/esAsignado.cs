using api.Methods;
using MySqlConnector;
using api.DTO;

namespace api.Endpoints;

public static class EsAsignadoEndpoints
{
    public static void MapEsAsignadoEndpoints(this WebApplication app)
    {
        app.MapGet("/asignados", async (IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT identificadorDispositivo, identificadorEstadio, identificadorPartido, identificadorSector, fecha
                FROM EsAsignado;
            """;

            var asignados = new List<object>();

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                asignados.Add(new
                {
                    IdentificadorDispositivo = reader.GetInt32("identificadorDispositivo"),
                    IdentificadorEstadio = reader.GetInt32("identificadorEstadio"),
                    IdentificadorPartido = reader.GetInt32("identificadorPartido"),
                    IdentificadorSector = reader.GetInt32("identificadorSector"),
                    Fecha = reader.GetDateTime("fecha")
                });
            }

            return Results.Ok(asignados);
        }).RequireAuthorization("SoloAdministrador");

        app.MapPost("/nuevoAsignado", async (IConfiguration config, EsAsignadoRequest request) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO EsAsignado (identificadorDispositivo, identificadorSector, identificadorEstadio, identificadorPartido) VALUES
                (@identificadorDispositivo, @identificadorSector, @identificadorEstadio, @identificadorPartido);
                """;

            command.Parameters.AddWithValue("@identificadorDispositivo", request.IdentificadorDispositivo);
            command.Parameters.AddWithValue("@identificadorSector", request.IdentificadorSector);
            command.Parameters.AddWithValue("@identificadorEstadio", request.IdentificadorEstadio);
            command.Parameters.AddWithValue("@identificadorPartido", request.IdentificadorPartido);

            try
            {
                await command.ExecuteNonQueryAsync();
            }
            catch (MySqlException ex)
            {
                return Results.Conflict(new
                {
                    success = false
                });
            }

            return Results.Ok(new
            {
                success = true,
                message = "El funcionario ha sido asignado correctamente"
            });
        }).RequireAuthorization("SoloAdministrador");

        app.MapPost("/asignado/borrar", async (IConfiguration config, EsAsignadoDeleteRequest request) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                DELETE FROM `EsAsignado`
                WHERE 
                    `identificadorDispositivo` = @identificadorDispositivo AND
                    `identificadorSector` = @identificadorSector AND
                    `identificadorEstadio` = @identificadorEstadio AND
                    `identificadorPartido` = @identificadorPartido;
                """;

            command.Parameters.AddWithValue("@identificadorDispositivo", request.IdentificadorDispositivo);
            command.Parameters.AddWithValue("@identificadorSector", request.IdentificadorSector);
            command.Parameters.AddWithValue("@identificadorEstadio", request.IdentificadorEstadio);
            command.Parameters.AddWithValue("@identificadorPartido", request.IdentificadorPartido);

            try
            {
                await command.ExecuteNonQueryAsync();
            }
            catch (MySqlException ex)
            {
                return Results.Conflict(new
                {
                    success = false
                });
            }

            return Results.Ok(new
            {
                success = true,
                message = "La asignación ha sido eliminada correctamente"
            });
        }).RequireAuthorization("SoloAdministrador");
    }
}