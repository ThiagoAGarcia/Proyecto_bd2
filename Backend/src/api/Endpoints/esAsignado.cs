using api.Methods;
using MySqlConnector;
using api.DTO;

namespace api.Endpoints;

public static class EsAsignadoEndpoints
{
    public static void MapEsAsignadoEndpoints(this WebApplication app)
    {
        app.MapGet("/allAsignados/{identificadorEstadio}/{identificadorSector}/{identificadorPartido}", async (IConfiguration config, int identificadorEstadio, int identificadorSector, int identificadorPartido) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT e.identificadorDispositivo, e.identificadorEstadio, e.identificadorPartido, e.identificadorSector, e.fecha, d.mailFuncionario
                FROM EsAsignado e
                JOIN Dispositivo d ON e.identificadorDispositivo = d.identificador
                WHERE e.identificadorEstadio = @identificadorEstadio AND e.identificadorSector = @identificadorSector AND e.identificadorPartido = @identificadorPartido;
            """;

            command.Parameters.AddWithValue("@identificadorEstadio", identificadorEstadio);
            command.Parameters.AddWithValue("@identificadorSector", identificadorSector);
            command.Parameters.AddWithValue("@identificadorPartido", identificadorPartido);

            await using var reader = await command.ExecuteReaderAsync();

            var asignados = new List<object>();

            while (await reader.ReadAsync())
            {
                asignados.Add(new
                {
                    IdentificadorDispositivo = reader.GetInt32("identificadorDispositivo"),
                    IdentificadorEstadio = reader.GetInt32("identificadorEstadio"),
                    IdentificadorPartido = reader.GetInt32("identificadorPartido"),
                    IdentificadorSector = reader.GetInt32("identificadorSector"),
                    Fecha = reader.GetDateTime("fecha"),
                    MailFuncionario = reader.GetString("mailFuncionario")
                });
            }

            return Results.Ok(asignados);
        }).RequireAuthorization("SoloAdministrador");

        app.MapGet("/allNoAsignados", async (IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT d.mailFuncionario, d.identificador
                FROM Dispositivo d
                LEFT JOIN EsAsignado e ON d.identificador = e.identificadorDispositivo
                WHERE e.identificadorDispositivo IS NULL AND d.mailFuncionario IS NOT NULL;
            """;

            await using var reader = await command.ExecuteReaderAsync();

            var noAsignados = new List<object>();

            while (await reader.ReadAsync())
            {
                noAsignados.Add(new
                {
                    MailFuncionario = reader.GetString("mailFuncionario"),
                    Identificador = reader.GetInt32("identificador")
                });
            }

            return Results.Ok(noAsignados);
        }).RequireAuthorization("SoloAdministrador");

        app.MapPost("/nuevoAsignado", async (IConfiguration config, EsAsignadoRequest request) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            try
            {
                await using (var deleteCommand = connection.CreateCommand())
                {
                    deleteCommand.CommandText = """
                        DELETE FROM EsAsignado
                        WHERE identificadorSector = @identificadorSector AND identificadorEstadio = @identificadorEstadio AND identificadorPartido = @identificadorPartido;
                    """;

                    deleteCommand.Parameters.AddWithValue("@identificadorSector", request.IdentificadorSector);
                    deleteCommand.Parameters.AddWithValue("@identificadorEstadio", request.IdentificadorEstadio);
                    deleteCommand.Parameters.AddWithValue("@identificadorPartido", request.IdentificadorPartido);

                    await deleteCommand.ExecuteNonQueryAsync();
                }

                await using (var insertCommand = connection.CreateCommand())
                {
                    insertCommand.CommandText = """
                        INSERT INTO EsAsignado ( identificadorDispositivo, identificadorSector, identificadorEstadio, identificadorPartido ) 
                        VALUES ( @identificadorDispositivo, @identificadorSector, @identificadorEstadio, @identificadorPartido);
                    """;

                    insertCommand.Parameters.AddWithValue("@identificadorDispositivo", request.IdentificadorDispositivo);
                    insertCommand.Parameters.AddWithValue("@identificadorSector", request.IdentificadorSector);
                    insertCommand.Parameters.AddWithValue("@identificadorEstadio", request.IdentificadorEstadio);
                    insertCommand.Parameters.AddWithValue("@identificadorPartido", request.IdentificadorPartido);

                    await insertCommand.ExecuteNonQueryAsync();
                }
            }
            catch (MySqlException)
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