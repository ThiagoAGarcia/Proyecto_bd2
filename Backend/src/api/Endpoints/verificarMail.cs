using api.Methods;
using MySqlConnector;
using api.DTOs;

namespace api.Endpoints;

public static class VerificarMailEndpoints
{
    public static void MapVerificarMailEndpoints(this WebApplication app)
    {
        app.MapGet("/verificar-email", async (string token, IConfiguration config) =>
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Results.BadRequest("Token invalido");
            }

            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT `MailPerfil`, `FechaVencimiento`, `Usado`
                FROM `VerificacionMail`
                WHERE `Token` = @token
                LIMIT 1;
                """;

            command.Parameters.AddWithValue("@token", token);

            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return Results.BadRequest("Token invalido");
            }

            var mail = reader.GetString("MailPerfil");
            var fechaVencimiento = reader.GetDateTime("FechaVencimiento");
            var usado = reader.GetBoolean("Usado");

            await reader.CloseAsync();

            if (usado)
            {
                return Results.BadRequest("Este link ya fue usado");
            }

            if (DateTime.UtcNow > fechaVencimiento)
            {
                return Results.BadRequest("El link expiro");
            }

            await using var updateCommand = connection.CreateCommand();
            updateCommand.CommandText = """
                UPDATE `Usuario`
                SET `estadoVerificado` = 'verificado'
                WHERE `MailPerfil` = @mail;

                UPDATE `VerificacionMail`
                SET `Usado` = TRUE
                WHERE `Token` = @token;
                """;

            updateCommand.Parameters.AddWithValue("@mail", mail);
            updateCommand.Parameters.AddWithValue("@token", token);

            await updateCommand.ExecuteNonQueryAsync();

            return Results.Ok(new
            {
                message = "Cuenta verificada correctamente"
            });
        });
    }
}