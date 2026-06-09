using MySqlConnector;
using api.DTOs;
namespace api.Endpoints;


public static class PerfilEndpoints
{
    public static void MapPerfilEndpoints(this WebApplication app)
    {
        app.MapGet("/perfiles", async (IConfiguration config) =>
        {
            var perfiles = new List<PerfilResponse>();
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT `Mail`, `PaisDocumento`, `TipoDocumento`, `NumeroDocumento`,
                       `DireccionLocalidad`, `DireccionNumero`, `DireccionCodigoPostal`
                FROM `Perfil`;
                """;

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                perfiles.Add(MapPerfil(reader));
            }

            return Results.Ok(perfiles);
        }).RequireAuthorization("SoloAdministrador");

        app.MapGet("/perfiles/{mail}", async (string mail, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT `Mail`, `PaisDocumento`, `TipoDocumento`, `NumeroDocumento`,
                       `DireccionLocalidad`, `DireccionNumero`, `DireccionCodigoPostal`
                FROM `Perfil`
                WHERE `Mail` = @mail
                LIMIT 1;
                """;
            command.Parameters.AddWithValue("@mail", mail);

            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return Results.NotFound();
            }

            return Results.Ok(MapPerfil(reader));
        }).RequireAuthorization("SoloUsuario");

        app.MapPost("/perfil", async (PerfilRequest request, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO `Perfil`
                    (`Mail`, `PaisDocumento`, `TipoDocumento`, `NumeroDocumento`,
                     `DireccionLocalidad`, `DireccionNumero`, `DireccionCodigoPostal`)
                VALUES
                    (@mail, @paisDocumento, @tipoDocumento, @numeroDocumento,
                     @direccionLocalidad, @direccionNumero, @direccionCodigoPostal);
                """;
            AddPerfilParameters(command, request);

            await command.ExecuteNonQueryAsync();

            return Results.Created($"/perfiles/{request.Mail}", request);
        });

        app.MapPut("/perfil/{mail}", async (string mail, PerfilUpdateRequest request, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                UPDATE `Perfil`
                SET `PaisDocumento` = @paisDocumento,
                    `TipoDocumento` = @tipoDocumento,
                    `NumeroDocumento` = @numeroDocumento,
                    `DireccionLocalidad` = @direccionLocalidad,
                    `DireccionNumero` = @direccionNumero,
                    `DireccionCodigoPostal` = @direccionCodigoPostal
                WHERE `Mail` = @mail;
                """;
            command.Parameters.AddWithValue("@mail", mail);
            command.Parameters.AddWithValue("@paisDocumento", request.PaisDocumento);
            command.Parameters.AddWithValue("@tipoDocumento", request.TipoDocumento);
            command.Parameters.AddWithValue("@numeroDocumento", request.NumeroDocumento);
            command.Parameters.AddWithValue("@direccionLocalidad", request.DireccionLocalidad);
            command.Parameters.AddWithValue("@direccionNumero", request.DireccionNumero);
            command.Parameters.AddWithValue("@direccionCodigoPostal", request.DireccionCodigoPostal);

            var affectedRows = await command.ExecuteNonQueryAsync();

            return affectedRows == 0
                ? Results.NotFound()
                : Results.NoContent();
        });

        app.MapDelete("/perfil/{mail}", async (string mail, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                DELETE FROM `Perfil`
                WHERE `Mail` = @mail;
                """;
            command.Parameters.AddWithValue("@mail", mail);

            var affectedRows = await command.ExecuteNonQueryAsync();

            return affectedRows == 0
                ? Results.NotFound()
                : Results.NoContent();
        });
    }

    private static PerfilResponse MapPerfil(MySqlDataReader reader)
    {
        return new PerfilResponse(
            reader.GetString("Mail"),
            reader.GetString("PaisDocumento"),
            reader.GetString("TipoDocumento"),
            reader.GetInt32("NumeroDocumento"),
            reader.GetString("DireccionLocalidad"),
            reader.GetInt32("DireccionNumero"),
            reader.GetInt32("DireccionCodigoPostal")
        );
    }

    private static void AddPerfilParameters(MySqlCommand command, PerfilRequest request)
    {
        command.Parameters.AddWithValue("@mail", request.Mail);
        command.Parameters.AddWithValue("@paisDocumento", request.PaisDocumento);
        command.Parameters.AddWithValue("@tipoDocumento", request.TipoDocumento);
        command.Parameters.AddWithValue("@numeroDocumento", request.NumeroDocumento);
        command.Parameters.AddWithValue("@direccionLocalidad", request.DireccionLocalidad);
        command.Parameters.AddWithValue("@direccionNumero", request.DireccionNumero);
        command.Parameters.AddWithValue("@direccionCodigoPostal", request.DireccionCodigoPostal);
    }
}
