using api.Methods;
using MySqlConnector;
using api.DTO;

namespace api.Endpoints;

public static class DispositivoFuncionarioEndpoints
{
    public static void MapDispositivoFuncionarioEndpoints(this WebApplication app)
    {
        app.MapGet("/dispositivosAsignados", async (IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT dF.mailFuncionario, d.identificador
                FROM `DispositivoFuncionario` dF
                INNER JOIN `Dispositivo` d ON dF.identificadorDispositivo = d.identificador
                """;

            await using var reader = await command.ExecuteReaderAsync();

            var asignaciones = new List<object>();

            while (await reader.ReadAsync())
            {
                asignaciones.Add(new
                {
                    IdentificadorDispositivo = reader.GetInt32("identificador")
                });
            }

            if (asignaciones.Count == 0)
            {
                return Results.NotFound(new
                {
                    success = false,
                    message = "No existen dispositivos sin asignar"
                });
            }

            return Results.Ok(new
            {
                success = true,
                asignaciones = asignaciones
            });
        }).RequireAuthorization("SoloAdministrador");

        app.MapGet("/dispositivosNoAsignados", async (IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT d.identificador
                FROM `Dispositivo` d
                LEFT JOIN `DispositivoFuncionario` dF ON dF.identificadorDispositivo = d.identificador
                WHERE dF.mailFuncionario IS NULL;
                """;

            await using var reader = await command.ExecuteReaderAsync();

            var asignaciones = new List<object>();

            while (await reader.ReadAsync())
            {
                asignaciones.Add(new
                {
                    IdentificadorDispositivo = reader.GetInt32("identificador")
                });
            }

            if (asignaciones.Count == 0)
            {
                return Results.NotFound(new
                {
                    success = false,
                    message = "No existen dispositivos sin asignar"
                });
            }

            return Results.Ok(new
            {
                success = true,
                asignaciones = asignaciones
            });
        }).RequireAuthorization("SoloAdministrador");

        app.MapPost("/dispositivoAsignado", async (IConfiguration config, DispositivoFuncionarioRequest request) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO `DispositivoFuncionario` (`mailFuncionario`, `identificadorDispositivo`)
                VALUES (@mailFuncionario, @identificadorDispositivo);
                """;

            command.Parameters.AddWithValue("@mailFuncionario", request.MailFuncionario);
            command.Parameters.AddWithValue("@identificadorDispositivo", request.IdentificadorDispositivo);

            await command.ExecuteNonQueryAsync();

            return Results.Ok(new
            {
                success = true,
                message = "El dispositivo ha sido asignado correctamente"
            });
        }).RequireAuthorization("SoloAdministrador");
    }
}