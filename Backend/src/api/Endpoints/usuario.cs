using api.Methods;
using MySqlConnector;
using api.DTOs;

namespace api.Endpoints;

public static class UsuarioEndpoints
{
    public static void MapUsuarioEndpoints(this WebApplication app)
    {
        app.MapGet("/allUsers", async (IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT `MailPerfil`, `numeroDocumento`
                FROM `Usuario` JOIN `Perfil` ON `Usuario`.`MailPerfil` = `Perfil`.`Mail`
                WHERE `Usuario`.`estadoVerificado` = 'verificado';
                """;

            await using var reader = await command.ExecuteReaderAsync();

            var users = new List<object>();

            while (await reader.ReadAsync())
            {
                users.Add(new
                {
                    MailPerfil = Normalizar.NormalizarMethod(reader.GetString("MailPerfil")),
                    NumeroDocumento = reader.GetInt32("numeroDocumento")
                });
            }

            return Results.Ok(users);
        }).RequireAuthorization();

        app.MapPost("/usuario", async (UserRequest request, IConfiguration config) =>
        {
            var mail = Normalizar.NormalizarMethod(request.MailPerfil);

            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO `Usuario` (`MailPerfil`)
                VALUES (@mail);
                """;

            command.Parameters.AddWithValue("@mail", mail);


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

            return Results.Created($"/usuario/{mail}", new
            {
                MailPerfil = mail
            });
        });

        app.MapGet("/usuario/{mail}", async (string mail, IConfiguration config, HttpContext context) =>
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
                SELECT `MailPerfil`, `estadoVerificado`
                FROM `Usuario`
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
                EstadoVerificado = Normalizar.NormalizarMethod(reader.GetString("estadoVerificado"))
            });
        }).RequireAuthorization();

        app.MapPut("/usuario/{mail}", async (string mail, UserUpdateRequest request, IConfiguration config, HttpContext context) =>
        {
            mail = Normalizar.NormalizarMethod(mail);




            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                UPDATE `Usuario`
                SET `estadoVerificado` = @estadoVerificado
                WHERE `mailPerfil` = @mail;
                """;

            command.Parameters.AddWithValue("@mail", mail);
            command.Parameters.AddWithValue("@estadoVerificado", Normalizar.NormalizarMethod(request.EstadoVerificado.ToString()));

            var affectedRows = await command.ExecuteNonQueryAsync();

            return affectedRows == 0
                ? Results.NotFound()
                : Results.NoContent();
        });
    }
}