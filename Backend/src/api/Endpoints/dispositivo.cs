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
                var idxMail = reader.GetOrdinal("mailFuncionario");
                var idxFecha = reader.GetOrdinal("fechaAsignacion");

                dispositivos.Add(new
                {
                    Identificador = reader.GetInt32("identificador"),
                    MailFuncionario = reader.IsDBNull(idxMail) ? null : reader.GetString(idxMail),
                    FechaAsignacion = reader.IsDBNull(idxFecha) ? (DateTime?)null : reader.GetDateTime(idxFecha)
                });
            }

            return Results.Ok(dispositivos);
        }).RequireAuthorization("SoloAdministrador");

        app.MapGet("/dispositivosNoAsignados", async (IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT D.identificador
                FROM Dispositivo D
                LEFT JOIN Funcionario F on D.mailFuncionario = F.mailPerfil
                WHERE D.mailFuncionario is null;
            """;

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

        app.MapPost("/nuevoDispositivo", async (IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO `Dispositivo` (`identificador`)
                VALUES (null);
                """;

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

            await using var checkCommand = connection.CreateCommand();
            checkCommand.CommandText = """
                SELECT COUNT(*)
                FROM EsAsignado
                WHERE identificadorDispositivo = @identificador;
            """;

            checkCommand.Parameters.AddWithValue("@identificador", identificador);

            var asignaciones = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());

            if (asignaciones > 0)
            {
                return Results.BadRequest(new
                {
                    success = false,
                    description = "El dispositivo está asignado a un sector y partido. Debe desasignarlo antes de eliminarlo."
                });
            }

            await using var deleteCommand = connection.CreateCommand();
            deleteCommand.CommandText = """
                DELETE FROM Dispositivo
                WHERE identificador = @identificador;
            """;

            deleteCommand.Parameters.AddWithValue("@identificador", identificador);

            var rowsAffected = await deleteCommand.ExecuteNonQueryAsync();

            if (rowsAffected == 0)
            {
                return Results.NotFound(new
                {
                    success = false,
                    description = "Dispositivo no encontrado"
                });
            }

            return Results.Ok(new
            {
                success = true,
                description = "El dispositivo ha sido eliminado correctamente"
            });
        }).RequireAuthorization("SoloAdministrador");

        app.MapPut("/updateDispositivo/{dispositivo}/{mail}", async (int dispositivo, string mail, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var transaction = await connection.BeginTransactionAsync();

            try
            {
                await using (var clearCommand = connection.CreateCommand())
                {
                    clearCommand.Transaction = transaction;

                    clearCommand.CommandText = """
                        UPDATE Dispositivo
                        SET mailFuncionario = NULL, fechaAsignacion = NULL
                        WHERE mailFuncionario = @mail;
                    """;

                    clearCommand.Parameters.AddWithValue("@mail", mail);

                    await clearCommand.ExecuteNonQueryAsync();
                }

                await using (var updateCommand = connection.CreateCommand())
                {
                    updateCommand.Transaction = transaction;

                    updateCommand.CommandText = """
                        UPDATE Dispositivo
                        SET mailFuncionario = @mail, fechaAsignacion = CURRENT_DATE()
                        WHERE identificador = @identificador;
                    """;

                    updateCommand.Parameters.AddWithValue("@identificador", dispositivo);
                    updateCommand.Parameters.AddWithValue("@mail", mail);

                    var rowsAffected = await updateCommand.ExecuteNonQueryAsync();

                    if (rowsAffected == 0)
                    {
                        await transaction.RollbackAsync();

                        return Results.NotFound(new
                        {
                            message = "Dispositivo no encontrado"
                        });
                    }
                }

                await transaction.CommitAsync();

                return Results.Ok(new
                {
                    success = true,
                    message = "El dispositivo se ha editado correctamente"
                });
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }).RequireAuthorization("SoloAdministrador");
    }
}