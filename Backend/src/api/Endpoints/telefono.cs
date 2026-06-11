using api.Methods;
using MySqlConnector;
using api.DTOs;

namespace api.Endpoints;

public static class TelefonoEndpoints
{
    public static void MapTelefonoEndpoints(this WebApplication app)
    {
        app.MapPost("/telefono", async (TelefonosRequest request, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            foreach (var telefono in request.Telefonos)
            {
                await using var command = connection.CreateCommand();

                command.CommandText = """
                    INSERT INTO Telefono
                        (MailPerfil, Telefono)
                    VALUES
                        (@mail, @telefono);
                    """;

                command.Parameters.AddWithValue("@mail", telefono.MailPerfil);
                command.Parameters.AddWithValue("@telefono", telefono.Telefono);

                await command.ExecuteNonQueryAsync();
            }
            return Results.Ok();
        });

        app.MapGet("/telefono/{mail}", async (string mail, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT Telefono
                FROM Telefono
                WHERE MailPerfil = @mail;
            """;

            command.Parameters.AddWithValue("@mail", mail);

            var telefonos = new List<string>();

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                telefonos.Add(reader.GetString("Telefono"));
            }

            return telefonos.Count == 0 ? Results.NotFound(new { message = "No se encontraron teléfonos para ese mail" }) : Results.Ok(telefonos);
        });
    }
}