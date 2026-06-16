using api.Methods;
using MySqlConnector;
using api.DTO;

namespace api.Endpoints;

public static class LoginEndpoints
{
    public static void MapLoginEndpoints(this WebApplication app)
    {
        app.MapPost("/loginCheck", async (LoginRequest request, IConfiguration config, HttpResponse response, HttpContext context) =>
        {
            var mail = Normalizar.NormalizarMethod(request.MailPerfil);

            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            var existingLogin = await GetLogin(connection, mail);

            if (existingLogin is null)
            {
                return Results.NotFound("Login no encontrado");
            }

            var isCorrect = BCrypt.Net.BCrypt.Verify(request.Password, existingLogin.Password);

            if (!isCorrect)
            {
                return Results.Unauthorized();
            }

            var typeUser = await GetUserType(connection, mail);

            if (typeUser is null)
            {
                return Results.Problem("No se pudo determinar el tipo de usuario");
            }

            var userVerified = await User.CheckUserVerificado(mail, config, context);

            if (typeUser == "Usuario" && !userVerified)
            {
                return Results.Json(new
                {
                    success = false, // No sé si borrarlo por si acaso
                    message = "Usuario no verificado"
                }, statusCode: StatusCodes.Status401Unauthorized);
            }

            Token.SetToken(config, response, Normalizar.NormalizarMethod(existingLogin.MailPerfil), typeUser);

            return Results.Ok(new
            {
                success = true,
                message = "Login correcto",
                role = typeUser
            });
        });

        app.MapPost("/login", async (LoginRequest request, IConfiguration config) =>
        {
            var mail = Normalizar.NormalizarMethod(request.MailPerfil);
            var passwordError = PerfilValidation.ValidarPassword(request.Password);

            if (passwordError is not null)
            {
                return Results.BadRequest(passwordError);
            }

            var connectionString = config.GetConnectionString("DefaultConnection");
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO `Login` (`MailPerfil`, `Password`)
                VALUES (@mail, @password);
                """;

            command.Parameters.AddWithValue("@mail", mail);
            command.Parameters.AddWithValue("@password", hashedPassword);

            try
            {
                await command.ExecuteNonQueryAsync();
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                var message = ex.Message.Contains("PRIMARY")
                    ? "Correo ya usado"
                    : "Error de clave duplicada";

                return Results.Conflict(new
                {
                    message
                });
            }

            var tokenVerificacion = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("/", "")
                .Replace("+", "")
                .Replace("=", "");

            var fechaVencimiento = DateTime.UtcNow.AddMinutes(15);

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
            commandToken.Parameters.AddWithValue("@fechaVencimiento", fechaVencimiento);

            await commandToken.ExecuteNonQueryAsync();

            var urlVerificacion = $"http://localhost:5001/verificar-email?token={tokenVerificacion}";

            var enviado = await Mail.EnviarMail(
                config,
                mail,
                "Verificar cuenta",
                $"Haz click en el enlace para verificar tu cuenta: {urlVerificacion}",
                $"""
                <!DOCTYPE html>
                <html lang="es">
                <head>
                    <meta charset="UTF-8">
                    <title>Verificación de cuenta</title>
                </head>
                <body style="
                    margin:0;
                    padding:0;
                    background-color:#f3f4f6;
                    font-family:Arial, Helvetica, sans-serif;
                ">
                    <div style="
                        width:100%;
                        padding:40px 0;
                    ">
                        <div style="
                            max-width:600px;
                            margin:0 auto;
                            background:white;
                            border-radius:16px;
                            overflow:hidden;
                            box-shadow:0 8px 24px rgba(0,0,0,0.1);
                        ">

                            <div style="
                                background:linear-gradient(135deg,#2563eb,#1d4ed8);
                                color:white;
                                text-align:center;
                                padding:40px 20px;
                            ">
                                <h1 style="
                                    margin:0;
                                    font-size:32px;
                                ">
                                    Mundial UCU 2026
                                </h1>

                                <p style="
                                    margin-top:10px;
                                    font-size:18px;
                                    opacity:0.9;
                                ">
                                    Bienvenido a la plataforma oficial
                                </p>
                            </div>

                            <div style="
                                padding:40px;
                                color:#374151;
                            ">
                                <h2 style="
                                    margin-top:0;
                                    color:#111827;
                                ">
                                    Verificación de cuenta
                                </h2>

                                <p style="
                                    font-size:16px;
                                    line-height:1.7;
                                ">
                                    Gracias por registrarte en <strong>Mundial UCU 2026</strong>.
                                    Para activar tu cuenta y comenzar a utilizar todas las funcionalidades
                                    de la plataforma, debes verificar tu dirección de correo electrónico.
                                </p>

                                <p style="
                                    font-size:16px;
                                    line-height:1.7;
                                ">
                                    Haz clic en el siguiente botón para completar la verificación:
                                </p>

                                <div style="text-align:center; margin:40px 0;">
                                    <a href="{urlVerificacion}"
                                    style="
                                            background:#2563eb;
                                            color:white;
                                            text-decoration:none;
                                            padding:16px 32px;
                                            border-radius:10px;
                                            font-size:18px;
                                            font-weight:bold;
                                            display:inline-block;
                                    ">
                                        ✓ Verificar mi cuenta
                                    </a>
                                </div>

                                <p style="
                                    font-size:16px;
                                    color:#6b7280;
                                ">
                                    Este enlace expirará en <strong>15 minutos</strong>.
                                </p>

                                <p style="
                                    font-size:15px;
                                    color:#6b7280;
                                ">
                                    Si el botón no funciona, puedes copiar y pegar el siguiente enlace en tu navegador:
                                </p>

                                <div style="
                                    background:#f9fafb;
                                    border:1px solid #e5e7eb;
                                    padding:15px;
                                    border-radius:8px;
                                    word-break:break-all;
                                    font-size:14px;
                                    color:#2563eb;
                                ">
                                    {urlVerificacion}
                                </div>

                                <hr style="
                                    margin:35px 0;
                                    border:none;
                                    border-top:1px solid #e5e7eb;
                                ">

                                <p style="
                                    font-size:14px;
                                    color:#9ca3af;
                                    text-align:center;
                                ">
                                    Si no creaste una cuenta en Mundial UCU 2026,
                                    puedes ignorar este correo de forma segura.
                                </p>
                            </div>

                            <div style="
                                background:#f9fafb;
                                padding:20px;
                                text-align:center;
                                color:#6b7280;
                                font-size:13px;
                            ">
                                © 2026 Mundial UCU · Todos los derechos reservados
                            </div>

                        </div>
                    </div>
                </body>
                </html>
                """
            );

            if (!enviado)
            {
                return Results.Problem("No se pudo enviar el mail");
            }

            return Results.Created($"/login/{mail}", new
            {
                Success = true,
                MailPerfil = mail
            });
        });

        app.MapPost("/loginOtherUsers", async (LoginRequest request, IConfiguration config) =>
        {
            var mail = Normalizar.NormalizarMethod(request.MailPerfil);

            var connectionString = config.GetConnectionString("DefaultConnection");
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO `Login` (`MailPerfil`, `Password`)
                VALUES (@mail, @password);
                """;

            command.Parameters.AddWithValue("@mail", mail);
            command.Parameters.AddWithValue("@password", hashedPassword);


            try
            {
                await command.ExecuteNonQueryAsync();
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                var message = ex.Message.Contains("PRIMARY")
                    ? "Correo ya usado"
                    : "Error de clave duplicada";

                return Results.Conflict(new
                {
                    message
                });
            }

            return Results.Created($"/LoginOtherUsers/{mail}", new
            {
                MailPerfil = mail
            });
        });

        app.MapPost("/logout", (HttpResponse response) =>
        {
            Token.ClearToken(response);

            return Results.Ok(new
            {
                message = "Sesion cerrada correctamente"
            });
        });

        app.MapDelete("/login/{mail}", async (string mail, IConfiguration config, HttpResponse response, HttpContext context) =>
        {
            mail = Normalizar.NormalizarMethod(mail);
            var tokenMail = Normalizar.NormalizarMethod(Token.GetMailUser(context));

            if (tokenMail != mail)
            {
                return Results.Unauthorized();
            }

            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                DELETE FROM `Login`
                WHERE `MailPerfil` = @mail;
                """;

            command.Parameters.AddWithValue("@mail", mail);

            var affectedRows = await command.ExecuteNonQueryAsync();

            if (affectedRows == 0)
            {
                return Results.NotFound();
            }

            Token.ClearToken(response);

            return Results.NoContent();
        });
    }

    private static async Task<LoginRow?> GetLogin(MySqlConnection connection, string mail)
    {
        mail = Normalizar.NormalizarMethod(mail);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT `MailPerfil`, `Password`
            FROM `Login`
            WHERE `MailPerfil` = @mail
            LIMIT 1;
            """;

        command.Parameters.AddWithValue("@mail", mail);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new LoginRow(
            Normalizar.NormalizarMethod(reader.GetString("MailPerfil")),
            reader.GetString("Password")
        );
    }

    private static async Task<string?> GetUserType(MySqlConnection connection, string mail)
    {
        mail = Normalizar.NormalizarMethod(mail);

        if (await ExistsByMail(connection, "Usuario", mail))
        {
            return "Usuario";
        }

        if (await ExistsByMail(connection, "Administrador", mail))
        {
            return "Administrador";
        }

        if (await ExistsByMail(connection, "Funcionario", mail))
        {
            return "Funcionario";
        }

        return null;
    }

    private static async Task<bool> ExistsByMail(MySqlConnection connection, string tableName, string mail)
    {
        mail = Normalizar.NormalizarMethod(mail);

        await using var command = connection.CreateCommand();
        command.CommandText = $"""
            SELECT 1
            FROM `{tableName}`
            WHERE `MailPerfil` = @mail
            LIMIT 1;
            """;

        command.Parameters.AddWithValue("@mail", mail);

        var result = await command.ExecuteScalarAsync();
        return result is not null;
    }
}