using api.Methods;
using MySqlConnector;
using api.DTOs;

namespace api.Endpoints;

public static class HabilitaEndpoints
{
    public static void MapHabilitaEndpoints(this WebApplication app)
    {
        app.MapPost("/habilita", async (HabilitaRequest request, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO `habilita` (`identificadorEstadio`, `identificadorPartido`, `identificadorSector`)
                VALUES (@estadio, @partido, @sector);
                """;

            command.Parameters.AddWithValue("@estadio", request.IdentificadorEstadio);
            command.Parameters.AddWithValue("@partido", request.IdentificadorPartido);
            command.Parameters.AddWithValue("@sector", request.IdentificadorSector);

            await command.ExecuteNonQueryAsync();

            return Results.Created($"/habilita", new
            {
                IdentificadorPartido = request.IdentificadorPartido,
                IdentificadorSector = request.IdentificadorSector,
                IdentificadorEstadio = request.IdentificadorEstadio
            });

        }).RequireAuthorization("SoloAdministrador");

        app.MapGet("/allHabilita/{estadio}/{partido}", async (int estadio, int partido, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT h.identificadorSector, s.nombre, s.capMax, s.tarifaExtra, p.precio
                FROM habilita h
                JOIN sector s ON h.identificadorSector = s.identificador AND h.identificadorEstadio = s.identificadorEstadio
                JOIN partido p ON h.identificadorPartido = p.identificador
                WHERE h.identificadorEstadio = @estadio AND h.identificadorPartido = @partido
            """;

            command.Parameters.AddWithValue("@estadio", estadio);
            command.Parameters.AddWithValue("@partido", partido);

            await using var reader = await command.ExecuteReaderAsync();

            var sectores = new List<object>();

            while (await reader.ReadAsync())
            {
                sectores.Add(new
                {
                    Identificador = reader.GetInt32("identificadorSector"),
                    Nombre = reader.GetString("nombre"),
                    CapacidadMaxima = reader.GetInt32("capMax"),
                    TarifaExtra = reader.GetInt32("tarifaExtra"),
                    PrecioBase = reader.GetInt32("precio")
                });
            }

            if (sectores.Count == 0)
            {
                return Results.NotFound(new
                {
                    message = "No hay sectores habilitados"
                });
            }

            return Results.Ok(sectores);
        }).RequireAuthorization();
    }
}