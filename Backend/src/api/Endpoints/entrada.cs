using api.Methods;
using MySqlConnector;
using api.DTOs;

namespace api.Endpoints;

public static class EntradaEndpoints
{
    public static void MapEntradaEndpoints(this WebApplication app)
    {
        app.MapPost("/entrada", async (EntradasRequest request, IConfiguration config, HttpContext context) =>
        {
            var mail = Normalizar.NormalizarMethod(Token.GetMailUser(context));

            if (request.Entradas.Count > 5)
            {
                return Results.BadRequest(new { success = false, message = "No se pueden comprar más de 5 entradas" });
            }

            if (request.Entradas is null || request.Entradas.Count == 0)
            {
                return Results.BadRequest(new { success = false, message = "Debe enviar al menos una entrada" });
            }

            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var saleIdCommand = connection.CreateCommand();

            saleIdCommand.CommandText = """
                SELECT LAST_INSERT_ID(v.identificador)
                FROM Venta v
                ORDER BY v.identificador DESC
                LIMIT 1;
            """;

            var saleId = Convert.ToInt32(await saleIdCommand.ExecuteScalarAsync());
            await using var checkPartidoCommando = connection.CreateCommand();
            checkPartidoCommando.CommandText = """
                SELECT p.identificador
                FROM Partido p
                WHERE p.identificador = @identificadorPartido AND p.fechaHora >= CURRENT_TIMESTAMP();
            """;

            checkPartidoCommando.Parameters.AddWithValue("@identificadorPartido", request.Entradas[0].IdentificadorPartido);
            var partidoExists = await checkPartidoCommando.ExecuteScalarAsync();
            if (partidoExists == null)
            {
                return Results.BadRequest(new { success = false, message = "El partido no existe o ya ha terminado" });
            }

            await using var capacityCommand = connection.CreateCommand();

            capacityCommand.CommandText = """
                SELECT s.capMax, COUNT(e.identificador) AS vendidas
                FROM Sector s
                LEFT JOIN Entrada e ON e.identificadorSector = s.identificador AND e.identificadorEstadio = s.identificadorEstadio AND e.identificadorPartido = @identificadorPartido AND e.estadoEntrada <> 'Cancelada'
                WHERE s.identificador = @identificadorSector AND s.identificadorEstadio = @identificadorEstadio
                GROUP BY s.capMax;
            """;

            capacityCommand.Parameters.AddWithValue("@identificadorPartido", request.Entradas[0].IdentificadorPartido);
            capacityCommand.Parameters.AddWithValue("@identificadorSector", request.Entradas[0].IdentificadorSector);
            capacityCommand.Parameters.AddWithValue("@identificadorEstadio", request.Entradas[0].IdentificadorEstadio);

            await using var reader = await capacityCommand.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return Results.BadRequest(new { success = false, message = "Sector inexistente" });
            }

            var capacidad = reader.GetInt32("capMax");
            var vendidas = reader.GetInt32("vendidas");

            if (vendidas + request.Entradas.Count > capacidad)
            {
                return Results.BadRequest(new { success = false, message = "No hay lugares suficientes en ese sector" });
            }

            await reader.CloseAsync();

            foreach (var entrada in request.Entradas)
            {
                await using var ticketCommand = connection.CreateCommand();

                ticketCommand.CommandText = """
                    INSERT INTO Entrada (identificadorVenta, identificadorPartido, mailUsuarioTiene, identificadorSector, identificadorEstadio) VALUES
                    (@saleId, @identificadorPartido, @mailUsuarioTiene, @identificadorSector, @identificadorEstadio);
                """;

                ticketCommand.Parameters.AddWithValue("@saleId", saleId);
                ticketCommand.Parameters.AddWithValue("@identificadorPartido", entrada.IdentificadorPartido);
                ticketCommand.Parameters.AddWithValue("@mailUsuarioTiene", mail);
                ticketCommand.Parameters.AddWithValue("@identificadorSector", entrada.IdentificadorSector);
                ticketCommand.Parameters.AddWithValue("@identificadorEstadio", entrada.IdentificadorEstadio);

                await ticketCommand.ExecuteNonQueryAsync();
            }

            return Results.Ok(new
            {
                success = true,
                message = "Entrada creada exitosamente"
            });
        }).RequireAuthorization("SoloUsuario");

        app.MapGet("/allEntradas", async (IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT `identificador`, `identificadorVenta`, `identificadorPartido`, `mailUsuarioTiene`, `estadoEntrada`, `identificadorSector`, `identificadorEstadio` 
                FROM `Entrada`;
                """;

            await using var reader = await command.ExecuteReaderAsync();

            var entradas = new List<object>();

            while (await reader.ReadAsync())
            {
                entradas.Add(new
                {
                    Identificador = reader.GetInt32("identificador"),
                    IdentificadorVenta = reader.GetInt32("identificadorVenta"),
                    IdentificadorPartido = reader.GetInt32("identificadorPartido"),
                    MailUsuarioTiene = reader.GetString("mailUsuarioTiene"),
                    EstadoEntrada = reader.GetString("estadoEntrada"),
                    IdentificadorSector = reader.GetInt32("identificadorSector"),
                    IdentificadorEstadio = reader.GetInt32("identificadorEstadio")
                });
            }

            return Results.Ok(entradas);
        }).RequireAuthorization();

        app.MapGet("/allMyEntradas", async (IConfiguration config, HttpContext context) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            var tokenMail = Normalizar.NormalizarMethod(Token.GetMailUser(context));

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT e.identificador, p.EquipoLocal, p.EquipoVisitante, el.bandera AS BanderaEquipoLocal, ev.bandera AS BanderaEquipoVisitante, p.fechaHora, e2.nombre AS NombreEstadio, s.nombre AS NombreSector
                FROM Entrada e
                JOIN Sector s ON e.identificadorSector = s.identificador AND e.identificadorEstadio = s.identificadorEstadio
                JOIN Partido p ON e.identificadorPartido = p.identificador
                JOIN Estadio e2 ON p.identificadorEstadio = e2.identificador
                JOIN Equipo el ON p.EquipoLocal = el.nombre
                JOIN Equipo ev ON p.EquipoVisitante = ev.nombre
                WHERE e.mailUsuarioTiene = @mailUsuarioTiene AND e.estadoEntrada = 'No registrada';
            """;

            command.Parameters.AddWithValue("@mailUsuarioTiene", tokenMail);
            await using var reader = await command.ExecuteReaderAsync();

            var entradas = new List<object>();

            while (await reader.ReadAsync())
            {
                entradas.Add(new
                {
                    Identificador = reader.GetInt32("identificador"),
                    EquipoLocal = reader.GetString("EquipoLocal"),
                    EquipoVisitante = reader.GetString("EquipoVisitante"),
                    BanderaEquipoLocal = reader.GetString("BanderaEquipoLocal"),
                    BanderaEquipoVisitante = reader.GetString("BanderaEquipoVisitante"),
                    FechaHora = reader.GetDateTime("fechaHora"),
                    NombreEstadio = reader.GetString("NombreEstadio"),
                    NombreSector = reader.GetString("NombreSector")
                });
            }

            return Results.Ok(entradas);
        }).RequireAuthorization();
    }
}