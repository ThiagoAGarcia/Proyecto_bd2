using api.Methods;
using MySqlConnector;
using api.DTOs;

namespace api.Endpoints;

public static class TransferenciaEndpoints
{
    public static void MapTransferenciaEndpoints(this WebApplication app)
    {
        app.MapPost("/transferencia", async (TransferenciaRequest request, IConfiguration config, HttpContext context) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            var mailOrigen = Normalizar.NormalizarMethod(Token.GetMailUser(context));
            var mailDestino = Normalizar.NormalizarMethod(request.MailUsuarioDestino);
            var identificador = request.IdentificadorEntrada;

            if (mailOrigen == mailDestino)
            {
                return Results.BadRequest(new
                {
                    Message = "No puede transferir una entrada a su propia cuenta."
                });
            }

            await using (var entradaCommand = connection.CreateCommand())
            {
                entradaCommand.CommandText = """
                    SELECT COUNT(*)
                    FROM Entrada
                    WHERE identificador = @identificador AND mailUsuarioTiene = @mailOrigen;
                """;

                entradaCommand.Parameters.AddWithValue("@identificador", identificador);
                entradaCommand.Parameters.AddWithValue("@mailOrigen", mailOrigen);

                var tieneEntrada = Convert.ToInt32(
                    await entradaCommand.ExecuteScalarAsync()
                );

                if (tieneEntrada == 0)
                {
                    return Results.BadRequest(new
                    {
                        Message = "Usted no es propietario de la entrada."
                    });
                }
            }

            await using (var countCommand = connection.CreateCommand())
            {
                countCommand.CommandText = """
                    SELECT COUNT(*)
                    FROM Transferencia
                    WHERE identificadorEntrada = @identificador;
                """;

                countCommand.Parameters.AddWithValue("@identificador", identificador);

                var cantidadTransferencias = Convert.ToInt32(
                    await countCommand.ExecuteScalarAsync()
                );

                if (cantidadTransferencias >= 3)
                {
                    return Results.BadRequest(new
                    {
                        Message = "La entrada ya alcanzó el máximo de 3 transferencias."
                    });
                }
            }

            await using (var estadoCommand = connection.CreateCommand())
            {
                estadoCommand.CommandText = """
                    SELECT estadoEntrada
                    FROM Entrada
                    WHERE identificador = @identificador;
                """;

                estadoCommand.Parameters.AddWithValue("@identificador", identificador);

                var estado = (await estadoCommand.ExecuteScalarAsync())?.ToString();

                if (estado == "Registrada" || estado == "Cancelada")
                {
                    return Results.BadRequest(new
                    {
                        Message = "La entrada no puede transferirse en su estado actual."
                    });
                }
            }

            await using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO Transferencia ( identificadorEntrada, mailUsuarioRealiza, mailUsuarioRecibe )
                VALUES ( @identificador, @mailRealiza, @mailRecibe );
            """;

            command.Parameters.AddWithValue("@identificador", identificador);
            command.Parameters.AddWithValue("@mailRealiza", mailOrigen);
            command.Parameters.AddWithValue("@mailRecibe", mailDestino);

            await using (var updateCommand = connection.CreateCommand())
            {
                updateCommand.CommandText = """
                    UPDATE Entrada
                    SET mailUsuarioTiene = @mailDestino
                    WHERE identificador = @identificador;
                """;

                updateCommand.Parameters.AddWithValue("@mailDestino", mailDestino);
                updateCommand.Parameters.AddWithValue("@identificador", identificador);

                await updateCommand.ExecuteNonQueryAsync();
            }

            try
            {
                await command.ExecuteNonQueryAsync();

                return Results.Ok(new
                {
                    success = true,
                    Message = "Transferencia realizada con éxito"
                });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new
                {
                    Message = "Error al realizar la transferencia",
                    Details = ex.Message
                });
            }
        }).RequireAuthorization("soloUsuario");

        app.MapGet("/allTransferencias", async (IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT `identificador`, `identificadorEntrada`, `mailUsuarioRealiza`, `mailUsuarioRecibe`, `fechaHora`
                FROM `Transferencia`
                """;

            await using var reader = await command.ExecuteReaderAsync();

            var transferencias = new List<object>();

            while (await reader.ReadAsync())
            {
                transferencias.Add(new
                {
                    Identificador = reader.GetInt32("identificador"),
                    IdentificadorEntrada = reader.GetInt32("identificadorEntrada"),
                    MailUsuarioRealiza = reader.GetString("mailUsuarioRealiza"),
                    MailUsuarioRecibe = reader.GetString("mailUsuarioRecibe"),
                    FechaHora = reader.GetDateTime("fechaHora")
                });
            }

            return Results.Ok(transferencias);
        }).RequireAuthorization();

        app.MapGet("/allMyTransferencias", async (IConfiguration config, HttpContext context) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            var tokenMail = Normalizar.NormalizarMethod(Token.GetMailUser(context));

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT t.identificador, t.identificadorEntrada, p.EquipoLocal, p.EquipoVisitante, el.bandera AS BanderaEquipoLocal, ev.bandera AS BanderaEquipoVisitante, t.mailUsuarioRealiza, t.mailUsuarioRecibe, t.fechaHora
                FROM Transferencia t
                JOIN Entrada e ON t.identificadorEntrada = e.identificador
                JOIN Partido p ON e.identificadorPartido = p.identificador
                JOIN Equipo el ON p.EquipoLocal = el.nombre
                JOIN Equipo ev ON p.EquipoVisitante = ev.nombre
                WHERE t.mailUsuarioRealiza = @mailUsuarioRealiza
            """;
            command.Parameters.AddWithValue("@mailUsuarioRealiza", tokenMail);

            await using var reader = await command.ExecuteReaderAsync();

            var transferencias = new List<object>();

            while (await reader.ReadAsync())
            {
                transferencias.Add(new
                {
                    Identificador = reader.GetInt32("identificador"),
                    IdentificadorEntrada = reader.GetInt32("identificadorEntrada"),
                    EquipoLocal = reader.GetString("EquipoLocal"),
                    EquipoVisitante = reader.GetString("EquipoVisitante"),
                    BanderaEquipoLocal = reader.GetString("BanderaEquipoLocal"),
                    BanderaEquipoVisitante = reader.GetString("BanderaEquipoVisitante"),
                    MailUsuarioRealiza = reader.GetString("mailUsuarioRealiza"),
                    MailUsuarioRecibe = reader.GetString("mailUsuarioRecibe"),
                    FechaHora = reader.GetDateTime("fechaHora")
                });
            }

            return Results.Ok(transferencias);
        }).RequireAuthorization("soloUsuario");
    }
}