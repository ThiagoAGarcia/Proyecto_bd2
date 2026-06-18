using api.Methods;
using MySqlConnector;
using api.DTO;

namespace api.Endpoints;

public static class DispositivoEndpoints
{
    public static void MapDispositivoEndpoints(this WebApplication app)
    {
        app.MapGet("/allDispositivos", async (IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT `identificador`, `mailFuncionario`, `fechaAsignacion`
                FROM `Dispositivo`;
                """;

            await using var reader = await command.ExecuteReaderAsync();

            var dispositivos = new List<Object>();

            while (await reader.ReadAsync())
            {
                dispositivos.Add(new
                {
                    Identificador = reader.GetInt32("identificador"),
                    MailFuncionario = reader.GetString("mailFuncionario"),
                    FechaAsignacion = reader.GetDateTime("fechaAsignacion")
                });
            }

            return Results.Ok(dispositivos);
        }).RequireAuthorization("SoloAdministrador");

        app.MapGet("/dispositivo/{identificador}", async (IConfiguration config, int identificador) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT `identificador`
                FROM `Dispositivo`
                WHERE `identificador` = @identificador;
                """;

            command.Parameters.AddWithValue("@identificador", identificador);

            await using var reader = await command.ExecuteReaderAsync();

            var dispositivos = new List<Object>();

            while (await reader.ReadAsync())
            {
                dispositivos.Add(new
                {
                    Identificador = reader.GetInt32("identificador")
                });
            }

            return Results.Ok(dispositivos);
        }).RequireAuthorization("SoloAdministrador");

        app.MapPost("/nuevoDispositivo", async (IConfiguration config, DispositivoRequest request) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO `Dispositivo` (`identificador`)
                VALUES (@identificador);
                """;

            command.Parameters.AddWithValue("@identificador", request.Identificador);

            await command.ExecuteNonQueryAsync();

            return Results.Ok(new
            {
                success = true,
                message = "El dispositivo ha sido creado correctamente"
            });
        }).RequireAuthorization("SoloAdministrador");

        app.MapDelete("/dispositivo/borrar/{identificador}", async (int identificador, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                DELETE FROM `Dispositivo`
                WHERE `identificador` = @identificador;
                """;

            command.Parameters.AddWithValue("@identificador", identificador);

            var rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected == 0)
            {
                return Results.NotFound(new
                {
                    message = "Dispositivo no encontrado"
                });
            }

            return Results.Ok(new
            {
                success = true,
                message = "El dispositivo ha sido eliminado correctamente"
            });
        }).RequireAuthorization("SoloAdministrador");

        app.MapPut("/dispositivo/editar/{identificador}", async (int identificador, IConfiguration config, DispositivoUpdateRequest request) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                UPDATE `Dispositivo`
                SET `identificador` = @identificadorNuevo
                WHERE `identificador` = @identificador;
                """;

            command.Parameters.AddWithValue("@identificador", identificador);
            command.Parameters.AddWithValue("@identificadorNuevo", request.IdentificadorNuevo);

            var rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected == 0)
            {
                return Results.NotFound(new
                {
                    message = "Dispositivo no encontrado"
                });
            }

            return Results.Ok(new
            {
                success = true,
                message = "El dispositivo se ha editado correctamente"
            });
        }).RequireAuthorization("SoloAdministrador");
    }
}