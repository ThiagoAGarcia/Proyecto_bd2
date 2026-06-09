using api.Methods;
using MySqlConnector;
using api.DTOs;

namespace api.Endpoints;

public static class LoginEndpoints
{
    public static void MapLoginEndpoints(this WebApplication app)
    {
        app.MapPost("/loginCheck", async (LoginRequest request, IConfiguration config, HttpResponse response) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            var existingLogin = await GetLogin(connection, request.MailPerfil);

            if (existingLogin is null)
            {
                return Results.NotFound("Login no encontrado");
            }

            var isCorrect = BCrypt.Net.BCrypt.Verify(request.Password, existingLogin.Password);

            if (!isCorrect)
            {
                return Results.Unauthorized();
            }

            var typeUser = await GetUserType(connection, request.MailPerfil);

            if (typeUser is null)
            {
                return Results.Problem("No se pudo determinar el tipo de usuario");
            }

            Token.SetToken(config, response, existingLogin.MailPerfil, typeUser);

            return Results.Ok(new
            {
                message = "Login correcto",
                role = typeUser
            });
        });

        app.MapPost("/login", async (LoginRequest request, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO `Login` (`MailPerfil`, `Password`)
                VALUES (@mail, @password);
                """;
            command.Parameters.AddWithValue("@mail", request.MailPerfil);
            command.Parameters.AddWithValue("@password", hashedPassword);

            await command.ExecuteNonQueryAsync();

            return Results.Created($"/login/{request.MailPerfil}", new
            {
                request.MailPerfil
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
            reader.GetString("MailPerfil"),
            reader.GetString("Password")
        );
    }

    private static async Task<string?> GetUserType(MySqlConnection connection, string mail)
    {
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

