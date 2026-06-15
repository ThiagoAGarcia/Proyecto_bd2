using api.DTOs;
using api.Methods;
using MySqlConnector;

namespace api.Endpoints;

public static class VentaEndpoints
{
    public static void MapVentaEndpoints(this WebApplication app)
    {
        app.MapPost("/user/nuevaVenta", async (VentaRequest request, IConfiguration config, HttpContext context) =>
        {
            var mail = Normalizar.NormalizarMethod(Token.GetMailUser(context));
            if (string.IsNullOrEmpty(mail))
            {
                return Results.BadRequest("El mail no puede ser nulo o vacío");
            }

            if (request.Entradas.Count > 5)
            {
                return Results.BadRequest("No se pueden comprar más de 5 entradas en una misma transacción");
            }

            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();

            command.CommandText = """
                INSERT INTO `venta` (`PorcentajeComision`, `MontoTotal`, `MailUsuarioComprado`)
                VALUES (@porcentajeComision, @montoTotal, @mail);
            """;

            command.Parameters.AddWithValue("@porcentajeComision", request.PorcentajeComision);
            command.Parameters.AddWithValue("@montoTotal", request.MontoTotal);
            command.Parameters.AddWithValue("@mail", mail);

            await command.ExecuteNonQueryAsync();

            await using var saleIdCommand = connection.CreateCommand();

            saleIdCommand.CommandText = """
                SELECT LAST_INSERT_ID(v.identificador)
                FROM Venta v
                ORDER BY v.identificador DESC
                LIMIT 1;
            """;


            var saleId = Convert.ToInt32(await saleIdCommand.ExecuteScalarAsync());

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
                ticketCommand.Parameters.AddWithValue("identificadorEstadio", entrada.IdentificadorEstadio);

                await ticketCommand.ExecuteNonQueryAsync();
            };

            return Results.Ok(new
            {
                success = true,
                message = "La venta se ha realizado con éxito"
            });
        }).RequireAuthorization("SoloUsuario");

        app.MapGet("/allVentas", async (IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT `identificador`, `fecha`, `porcentajeComision`, `montoTotal`, `mailUsuarioComprado`
                FROM `Venta`;
                """;

            await using var reader = await command.ExecuteReaderAsync();

            var ventas = new List<object>();

            while (await reader.ReadAsync())
            {
                ventas.Add(new
                {
                    Identificador = reader.GetInt32("identificador"),
                    Fecha = reader.GetDateTime("fecha"),
                    PorcentajeComision = reader.GetInt32("porcentajeComision"),
                    MontoTotal = reader.GetInt32("montoTotal"),
                    MailUsuarioComprado = reader.GetString("mailUsuarioComprado")
                });
            }

            return Results.Ok(ventas);
        }).RequireAuthorization();
    }
}