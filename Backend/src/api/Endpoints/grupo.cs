using api.Methods;
using MySqlConnector;
using api.DTOs;

namespace api.Endpoints;

public static class GrupoEndpoints
{
    public static void MapGrupoEndpoints(this WebApplication app)
    {
        app.MapPost("/grupo", async (GrupoRequest request, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            var NombreGrupo = Normalizar.NormalizarMethod(request.NombreGrupo);
            var NombreEtapa = Normalizar.NormalizarMethod(request.NombreEtapa);

            await using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO `grupo` (`NombreGrupo`, `NombreEtapa`)
                VALUES (@nombreGrupo, @nombreEtapa);
                """;

            command.Parameters.AddWithValue("@nombreGrupo", NombreGrupo);
            command.Parameters.AddWithValue("@nombreEtapa", NombreEtapa);

            await command.ExecuteNonQueryAsync();

            return Results.Created($"/grupo", new
            {
                NombreGrupo = request.NombreGrupo,
                NombreEtapa = request.NombreEtapa
            });
        }).RequireAuthorization("SoloAdministrador");
        app.MapGet("/grupo/{nomreGrupo}", async (string nombreGrupo, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
            SELECT `NombreGrupo`, `NombreEtapa`
            FROM `grupo`
            WHERE `NombreGrupo` = @nombreGrupo
            LIMIT 1;
            """;

            command.Parameters.AddWithValue("@nombreGrupo", nombreGrupo);

            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return Results.NotFound();
            }

            return Results.Ok(new
            {
                NombreGrupo = reader.GetString("NombreGrupo"),
                NombreEtapa = reader.GetString("NombreEtapa")
            });
        }).RequireAuthorization();
        app.MapGet("/allGrupo", async (IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT `NombreGrupo`, `NombreEtapa`
                FROM `grupo`;
                """;

            await using var reader = await command.ExecuteReaderAsync();

            var grupos = new List<object>();

            while (await reader.ReadAsync())
            {
                grupos.Add(new
                {
                    NombreGrupo = reader.GetString("NombreGrupo"),
                    NombreEtapa = reader.GetString("NombreEtapa")
                });
            }

            return Results.Ok(grupos);
        }).RequireAuthorization();

        app.MapDelete("/grupo/{nombreGrupo}", async (string nombreGrupo, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                DELETE FROM `grupo`
                WHERE `NombreGrupo` = @nombreGrupo;
                """;

            command.Parameters.AddWithValue("@nombreGrupo", nombreGrupo);

            var affectedRows = await command.ExecuteNonQueryAsync();

            if (affectedRows == 0)
            {
                var message = "Grupo no encontrado";
                return Results.NotFound(message);
            }

            return Results.Ok("Grupo eliminado exitosamente");
        }).RequireAuthorization("SoloAdministrador");
    }
}