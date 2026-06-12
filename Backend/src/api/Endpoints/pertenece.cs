using api.Methods;
using MySqlConnector;
using api.DTOs;

namespace api.Endpoints;

public static class EquipoPerteneceGrupoEndpoints
{
    public static void MapEquipoPerteneceGrupoEndpoints(this WebApplication app)
    {
        app.MapPost("/EquipoPerteneceGrupo", async (PerteneceRequest request, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            var NombreEquipo = Normalizar.NormalizarMethod(request.NombreEquipo);
            var NombreGrupo = Normalizar.NormalizarMethod(request.NombreGrupo);
            var NombreEtapa = Normalizar.NormalizarMethod(request.NombreEtapa);

            await using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO `pertenece` (`NombreEquipo`, `NombreGrupo`, `NombreEtapa`)
                VALUES (@nombreEquipo, @nombreGrupo, @nombreEtapa);
                """;

            command.Parameters.AddWithValue("@nombreEquipo", NombreEquipo);
            command.Parameters.AddWithValue("@nombreGrupo", NombreGrupo);
            command.Parameters.AddWithValue("@nombreEtapa", NombreEtapa);

            await command.ExecuteNonQueryAsync();

            return Results.Created($"/EquipoPerteneceGrupo", new
            {
                NombreEquipo = request.NombreEquipo,
                NombreGrupo = request.NombreGrupo,
                NombreEtapa = request.NombreEtapa
            });
        }).RequireAuthorization("SoloAdministrador");

        app.MapGet("/EquipoPerteneceGrupo/{nombreEquipo}", async (string nombreEquipo, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT `NombreEquipo`, `NombreGrupo`, `NombreEtapa`
                FROM `pertenece`
                WHERE `NombreEquipo` = @nombreEquipo
                LIMIT 1;
                """;

            command.Parameters.AddWithValue("@nombreEquipo", nombreEquipo);

            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return Results.NotFound();
            }

            return Results.Ok(new
            {
                NombreEquipo = reader.GetString("NombreEquipo"),
                NombreGrupo = reader.GetString("NombreGrupo"),
                NombreEtapa = reader.GetString("NombreEtapa")
            });
        }).RequireAuthorization("SoloAdministrador");

        app.MapGet("/allEquipoPerteneceGrupo", async (IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT `NombreEquipo`, `NombreGrupo`, `NombreEtapa`
                FROM `pertenece`;
                """;

            await using var reader = await command.ExecuteReaderAsync();

            var resultados = new List<object>();

            while (await reader.ReadAsync())
            {
                resultados.Add(new
                {
                    NombreEquipo = reader.GetString("NombreEquipo"),
                    NombreGrupo = reader.GetString("NombreGrupo"),
                    NombreEtapa = reader.GetString("NombreEtapa")
                });
            }

            return Results.Ok(resultados);
        }).RequireAuthorization();

        app.MapGet("/getEquipoPerteneceGrupoByGrupo/{nombreGrupo}", async (string nombreGrupo, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT `NombreEquipo`, `NombreGrupo`, `NombreEtapa`
                FROM `pertenece`
                WHERE `NombreGrupo` = @nombreGrupo;
                """;

            command.Parameters.AddWithValue("@nombreGrupo", nombreGrupo);

            await using var reader = await command.ExecuteReaderAsync();

            var resultados = new List<object>();

            while (await reader.ReadAsync())
            {
                resultados.Add(new
                {
                    NombreEquipo = reader.GetString("NombreEquipo"),
                    NombreGrupo = reader.GetString("NombreGrupo"),
                    NombreEtapa = reader.GetString("NombreEtapa")
                });
            }

            return Results.Ok(resultados);
        }).RequireAuthorization();

        app.MapGet("/getEquipoPerteneceGrupoByEtapa/{nombreEtapa}", async (string nombreEtapa, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT `NombreEquipo`, `NombreGrupo`, `NombreEtapa`
                FROM `pertenece`
                WHERE `NombreEtapa` = @nombreEtapa;
                """;

            command.Parameters.AddWithValue("@nombreEtapa", nombreEtapa);

            await using var reader = await command.ExecuteReaderAsync();

            var resultados = new List<object>();

            while (await reader.ReadAsync())
            {
                resultados.Add(new
                {
                    NombreEquipo = reader.GetString("NombreEquipo"),
                    NombreGrupo = reader.GetString("NombreGrupo"),
                    NombreEtapa = reader.GetString("NombreEtapa")
                });
            }

            return Results.Ok(resultados);
        }).RequireAuthorization();

        app.MapDelete("/EquipoPerteneceGrupo/{nombreEquipo}&{nombreGrupo}&{nombreEtapa}", async (string nombreEquipo, string nombreGrupo, string nombreEtapa, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                DELETE FROM `pertenece`
                WHERE `NombreEquipo` = @nombreEquipo AND `NombreGrupo` = @nombreGrupo AND `NombreEtapa` = @nombreEtapa;
                """;

            command.Parameters.AddWithValue("@nombreEquipo", nombreEquipo);
            command.Parameters.AddWithValue("@nombreGrupo", nombreGrupo);
            command.Parameters.AddWithValue("@nombreEtapa", nombreEtapa);

            var affectedRows = await command.ExecuteNonQueryAsync();

            if (affectedRows == 0)
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        }).RequireAuthorization("SoloAdministrador");

    }
}