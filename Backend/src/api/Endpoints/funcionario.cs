using api.Methods;
using MySqlConnector;
using api.DTO;

namespace api.Endpoints;

public static class FuncionarioEndpoints
{
    public static void MapFuncionarioEndpoints(this WebApplication app)
    {
        app.MapPost("/funcionario", async (FuncionarioRequest request, IConfiguration config) =>
        {
            var mail = Normalizar.NormalizarMethod(request.MailPerfil);

            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO `funcionario` (`MailPerfil`, `NumeroLegajo`)
                VALUES (@mail, @numeroLegajo);
                """;

            command.Parameters.AddWithValue("@mail", mail);
            command.Parameters.AddWithValue("@numeroLegajo", request.NumeroLegajo);

            try
            {
                await command.ExecuteNonQueryAsync();
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                var message = ex.Message.Contains("PRIMARY")
                    ? "Correo ya usado"
                    : "Clave duplicada";

                return Results.Conflict(new
                {
                    message
                });
            }

            return Results.Created($"/funcionario/{mail}", new
            {
                success = true,
                MailPerfil = mail
            });
        }).RequireAuthorization("SoloAdministrador");

        app.MapGet("/funcionario/{mail}", async (string mail, IConfiguration config, HttpContext context) =>
        {
            mail = Normalizar.NormalizarMethod(mail);
            var tokenMail = Normalizar.NormalizarMethod(Token.GetMailUser(context));

            if (string.IsNullOrEmpty(mail))
            {
                return Results.BadRequest("El mail no puede ser nulo o vacío");
            }

            if (tokenMail != mail)
            {
                return Results.Unauthorized();
            }

            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT `MailPerfil`, `NumeroLegajo`
                FROM `funcionario`
                WHERE `MailPerfil` = @mail
                LIMIT 1;
                """;

            command.Parameters.AddWithValue("@mail", mail);

            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return Results.NotFound();
            }

            return Results.Ok(new
            {
                MailPerfil = Normalizar.NormalizarMethod(reader.GetString("MailPerfil")),
                NumeroLegajo = reader.GetInt32("NumeroLegajo")
            });
        }).RequireAuthorization("SoloAdministrador");

        app.MapGet("/allFuncionarios", async (IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT d.identificador, f.mailPerfil, f.numeroLegajo
                FROM Funcionario f
                LEFT JOIN Dispositivo d ON f.mailPerfil = d.mailFuncionario;
            """;

            await using var reader = await command.ExecuteReaderAsync();

            var funcionarios = new List<object>();

            var identificadorOrdinal = reader.GetOrdinal("identificador");

            while (await reader.ReadAsync())
            {
                funcionarios.Add(new
                {
                    Identificador = reader.IsDBNull(identificadorOrdinal) ? null : (int?)reader.GetInt32(identificadorOrdinal),
                    MailPerfil = Normalizar.NormalizarMethod(reader.GetString("MailPerfil")),
                    NumeroLegajo = reader.GetInt32("NumeroLegajo")
                });
            }

            return Results.Ok(funcionarios);
        }).RequireAuthorization("SoloAdministrador");


        app.MapDelete("/funcionarioPerfil/{mail}", async (string mail, IConfiguration config) =>
        {
            mail = Normalizar.NormalizarMethod(mail);

            if (string.IsNullOrEmpty(mail))
            {
                return Results.BadRequest("El mail no puede ser nulo o vacío");
            }

            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var transaction = await connection.BeginTransactionAsync();

            try
            {
                await using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = """
                        UPDATE dispositivo
                        SET mailFuncionario = NULL
                        WHERE mailFuncionario = @mail;
                    """;

                    command.Parameters.AddWithValue("@mail", mail);

                    await command.ExecuteNonQueryAsync();
                }

                await using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = """
                        DELETE FROM `funcionario`
                        WHERE `MailPerfil` = @mail;
                    """;

                    command.Parameters.AddWithValue("@mail", mail);

                    var rowsAffected = await command.ExecuteNonQueryAsync();

                    if (rowsAffected == 0)
                    {
                        await transaction.RollbackAsync();
                        return Results.NotFound();
                    }
                }

                await using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = """
                        DELETE FROM `login`
                        WHERE `MailPerfil` = @mail;
                    """;

                    command.Parameters.AddWithValue("@mail", mail);

                    await command.ExecuteNonQueryAsync();
                }

                await using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = """
                        DELETE FROM `telefono`
                        WHERE `MailPerfil` = @mail;
                    """;

                    command.Parameters.AddWithValue("@mail", mail);

                    await command.ExecuteNonQueryAsync();
                }

                await using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = """
                        DELETE FROM `perfil`
                        WHERE `Mail` = @mail;
                    """;

                    command.Parameters.AddWithValue("@mail", mail);

                    await command.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();

                return Results.Ok(new
                {
                    success = true,
                    MailPerfil = mail
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