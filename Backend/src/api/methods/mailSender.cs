namespace api.Methods;

using System.Net.Http.Headers;
using System.Text;
using MySqlConnector;

public static class Mail
{
    public static async Task<bool> EnviarMail(
    IConfiguration config,
    string destinatario,
    string asunto,
    string texto,
    string html)
    {
        var apiKey = config["Mailgun:ApiKey"];
        var domain = config["Mailgun:Domain"];
        var from = config["Mailgun:From"];

        using var client = new HttpClient();

        var authToken = Convert.ToBase64String(
            Encoding.ASCII.GetBytes($"api:{apiKey}")
        );

        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);

        using var content = new MultipartFormDataContent
    {
        { new StringContent(from!), "from" },
        { new StringContent(destinatario), "to" },
        { new StringContent(asunto), "subject" },
        { new StringContent(texto), "text" },
        { new StringContent(html), "html" }
    };

        var response = await client.PostAsync(
            $"https://api.mailgun.net/v3/{domain}/messages",
            content
        );

        return response.IsSuccessStatusCode;
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

// var enviado = await Mail.EnviarMail(
//     config,
//     "thiagoandresgg@gmail.com",
//     "Verificar cuenta",
//     "Haz click en el enlace para verificar tu cuenta: https://localhost:5001/verificar",
//     """
//     <html>
//         <body>
//             <h2>Verificar cuenta</h2>
//             <p>Haz click en el boton para verificar tu cuenta.</p>

//             <a href="https://localhost:5001/verificar"
//                style="
//                    display:inline-block;
//                    padding:12px 20px;
//                    background-color:#2563eb;
//                    color:white;
//                    text-decoration:none;
//                    border-radius:6px;
//                    font-weight:bold;
//                ">
//                 Verificar cuenta
//             </a>
//         </body>
//     </html>
//     """
// );

//             if (!enviado)
//             {
//                 return Results.Problem("No se pudo enviar el mail");
//             }