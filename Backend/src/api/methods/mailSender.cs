using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MySqlConnector;

namespace api.Methods;

public static class Mail
{
    public static async Task<bool> EnviarMail(
        IConfiguration config,
        string destinatario,
        string asunto,
        string texto,
        string html)
    {
        try
        {
            var email = config["Gmail:Email"];
            var password = config["Gmail:Password"];

            var message = new MimeMessage();

            message.From.Add(
                new MailboxAddress("Mundial UCU 2026", email)
            );

            message.To.Add(
                MailboxAddress.Parse(destinatario)
            );

            message.Subject = asunto;

            var bodyBuilder = new BodyBuilder
            {
                TextBody = texto,
                HtmlBody = html
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();

            await client.ConnectAsync(
                "smtp.gmail.com",
                587,
                SecureSocketOptions.StartTls
            );

            await client.AuthenticateAsync(
                email,
                password
            );

            await client.SendAsync(message);

            await client.DisconnectAsync(true);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error enviando mail: {ex.Message}");
            return false;
        }
    }

    public static async Task<string?> CrearTokenVerificacion(
        MySqlConnection connection,
        string mail)
    {
        var tokenVerificacion = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("/", "")
            .Replace("+", "")
            .Replace("=", "");

        var venceEn = DateTime.UtcNow.AddMinutes(15);

        await using var commandToken = connection.CreateCommand();
        commandToken.CommandText = """
        INSERT INTO `VerificacionMail` (`MailPerfil`, `Token`, `FechaVencimiento`, `Usado`)
        VALUES (@mail, @token, @fechaVencimiento, FALSE)
        ON DUPLICATE KEY UPDATE
            `Token` = @token,
            `FechaVencimiento` = @fechaVencimiento,
            `Usado` = FALSE;
        """;

        commandToken.Parameters.AddWithValue("@mail", mail);
        commandToken.Parameters.AddWithValue("@token", tokenVerificacion);
        commandToken.Parameters.AddWithValue("@fechaVencimiento", venceEn);

        await commandToken.ExecuteNonQueryAsync();

        return tokenVerificacion;
    }
}