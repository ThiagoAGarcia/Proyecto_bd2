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

        app.MapPut("/updateHabilita", async (UpdateHabilitaRequest request, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var transaction = await connection.BeginTransactionAsync();

            try
            {
                await using (var checkCommand = connection.CreateCommand())
                {
                    checkCommand.Transaction = transaction;

                    checkCommand.CommandText = """
                        SELECT COUNT(*)
                        FROM entrada
                        WHERE identificadorEstadio = @estadio AND identificadorPartido = @partido
                    """;

                    checkCommand.Parameters.AddWithValue("@estadio", request.IdentificadorEstadio);
                    checkCommand.Parameters.AddWithValue("@partido", request.IdentificadorPartido);

                    var cantidadEntradas = Convert.ToInt32(
                        await checkCommand.ExecuteScalarAsync()
                    );

                    if (cantidadEntradas > 0)
                    {
                        return Results.BadRequest(new
                        {
                            message = "No se pueden modificar los sectores porque existen entradas asociadas."
                        });
                    }
                }

                await using (var deleteCommand = connection.CreateCommand())
                {
                    deleteCommand.Transaction = transaction;

                    deleteCommand.CommandText = """
                        DELETE FROM habilita
                        WHERE identificadorEstadio = @estadio AND identificadorPartido = @partido
                    """;

                    deleteCommand.Parameters.AddWithValue("@estadio", request.IdentificadorEstadio);
                    deleteCommand.Parameters.AddWithValue("@partido", request.IdentificadorPartido);

                    await deleteCommand.ExecuteNonQueryAsync();
                }

                foreach (var sector in request.Sectores)
                {
                    await using var insertCommand = connection.CreateCommand();

                    insertCommand.Transaction = transaction;

                    insertCommand.CommandText = """
                        INSERT INTO habilita ( identificadorEstadio, identificadorPartido, identificadorSector ) VALUES
                        ( @estadio, @partido, @sector )
                    """;

                    insertCommand.Parameters.AddWithValue("@estadio", request.IdentificadorEstadio);
                    insertCommand.Parameters.AddWithValue("@partido", request.IdentificadorPartido);
                    insertCommand.Parameters.AddWithValue("@sector", sector);

                    await insertCommand.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();

                return Results.Ok(new
                {
                    success = true,
                    message = "Habilitaciones actualizadas correctamente"
                });
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
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