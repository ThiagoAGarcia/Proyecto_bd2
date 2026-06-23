using api.Methods;
using MySqlConnector;
using api.DTO;

namespace api.Endpoints;

public static class EstadisticasEndpoints
{
    public static void MapEstadisticasEndpoints(this WebApplication app)
    {
        app.MapGet("/AllPartidosConId", async (IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();

            command.CommandText = """
            SELECT
                identificador,
                fechaHora,
                EquipoLocal AS equipoLocal,
                EquipoVisitante AS equipoVisitante
            FROM Partido
            """;

            var partidosConId = new List<object>();

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                partidosConId.Add(new
                {
                    idPartido = reader.GetInt32("identificador"),
                    fechaHora = reader.GetDateTime("fechaHora"),
                    equipoLocal = reader.GetString("equipoLocal"),
                    equipoVisitante = reader.GetString("equipoVisitante")
                });
            }

            return Results.Ok(partidosConId);
        }).RequireAuthorization("SoloAdministrador");

        app.MapGet("/RankingCompradores", async (IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();

            command.CommandText = """
            SELECT mailUsuarioComprado as usuarioComprador, COUNT(*) as ventas 
            FROM venta 
            GROUP BY mailUsuarioComprado ORDER BY ventas DESC LIMIT 3;
            """;

            var rankingCompradores = new List<object>();

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                rankingCompradores.Add(new
                {
                    usuarioComprador = reader.GetString("usuarioComprador"),
                    ventas = reader.GetInt32("ventas")
                });
            }

            return Results.Ok(rankingCompradores);
        }).RequireAuthorization("SoloAdministrador");

        app.MapGet("/RankingPartidosMayorVendidos", async (IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();

            command.CommandText = """
            SELECT CONCAT(p.EquipoLocal, ' vs ', p.EquipoVisitante) as partido, COUNT(*) as cant_ventas 
            FROM Entrada e 
            JOIN Partido p ON p.identificador = e.identificadorPartido 
            GROUP BY p.identificador 
            ORDER BY cant_ventas DESC;
            """;

            var rankingPartidos = new List<object>();

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                rankingPartidos.Add(new
                {
                    partido = reader.GetString("partido"),
                    cant_ventas = reader.GetInt32("cant_ventas")
                });
            }

            return Results.Ok(rankingPartidos);
        }).RequireAuthorization("SoloAdministrador");

    }
}