using api.Methods;
using MySqlConnector;
using api.DTO;

namespace api.Endpoints;

public static class Fixture
{
    public static void MapFixtureEndpoints(this WebApplication app)
    {
        app.MapGet("/fixture/grupos", async (IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();

            command.CommandText = """
                SELECT
                    g.nombreGrupo,
                    g.nombreEtapa,
                    e.nombre,
                    e.bandera
                FROM Grupo g
                JOIN Pertenece p
                    ON p.nombreGrupo = g.nombreGrupo
                    AND p.nombreEtapa = g.nombreEtapa
                JOIN Equipo e
                    ON e.nombre = p.nombreEquipo
                ORDER BY g.nombreGrupo;
                """;

            await using var reader = await command.ExecuteReaderAsync();

            var grupos = new Dictionary<string, List<object>>();

            while (await reader.ReadAsync())
            {
                var grupo = reader.GetString("nombreGrupo");

                if (!grupos.ContainsKey(grupo))
                    grupos[grupo] = [];

                grupos[grupo].Add(new
                {
                    nombre = reader.GetString("nombre"),
                    bandera = reader.GetString("bandera")
                });
            }

            return Results.Ok(grupos);
        });
        app.MapGet("/fixture/partidos", async (IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();

            command.CommandText = """
                SELECT
                    identificador,
                    fase,
                    EquipoLocal,
                    EquipoVisitante,
                    fechaHora
                FROM Partido;
                """;

            await using var reader = await command.ExecuteReaderAsync();

            var partidos = new List<object>();

            while (await reader.ReadAsync())
            {
                partidos.Add(new
                {
                    Identificador = reader.GetInt32("identificador"),
                    Fase = reader.GetString("fase"),
                    EquipoLocal = reader.GetString("EquipoLocal"),
                    EquipoVisitante = reader.GetString("EquipoVisitante"),
                    FechaHora = reader.GetDateTime("fechaHora")
                });
            }

            return Results.Ok(partidos);
        });
    }
}