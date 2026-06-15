using api.Methods;
using MySqlConnector;
using api.DTOs;

namespace api.Endpoints;

public static class AdministradorEndpoints
{
    public static void MapAdministradorEndpoints(this WebApplication app)
    {
        app.MapPost("/administrador", async (AdministradorRequest request, IConfiguration config) =>
        {
            var mail = Normalizar.NormalizarMethod(request.MailPerfil);

            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO `administrador` (`MailPerfil`, `NombrePais`)
                VALUES (@mail, @nombrePais);
                """;

            command.Parameters.AddWithValue("@mail", mail);
            command.Parameters.AddWithValue("@nombrePais", request.NombrePais);

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

            return Results.Created($"/administrador/{mail}", new
            {
                MailPerfil = mail,
                NombrePais = request.NombrePais
            });
        }).RequireAuthorization("SoloAdministrador");

        app.MapGet("/administrador/{mail}", async (string mail, IConfiguration config, HttpContext context) =>
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
                SELECT `MailPerfil`, `NombrePais`
                FROM `administrador`
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
                NombrePais = reader.GetString("NombrePais")
            });
        }).RequireAuthorization();
    }
}