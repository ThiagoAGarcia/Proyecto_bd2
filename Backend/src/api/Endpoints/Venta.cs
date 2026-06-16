using api.DTO;
using api.Methods;
using MySqlConnector;

namespace api.Endpoints;

public static class VentaEndpoints
{
    public static void MapVentaEndpoints(this WebApplication app)
    {
        app.MapPost("/venta", async (VentaRequest request, IConfiguration config, HttpContext context) =>
        {
            var mail = Normalizar.NormalizarMethod(Token.GetMailUser(context));

            if (string.IsNullOrEmpty(mail))
            {
                return Results.BadRequest("El mail no puede ser nulo o vacío");
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

            return Results.Ok(new
            {
                success = true,
                message = "Venta creada exitosamente"
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

        app.MapGet("/allMyVentas", async (IConfiguration config, HttpContext context) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            var tokenMail = Normalizar.NormalizarMethod(Token.GetMailUser(context));

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT v.identificador, v.fecha, p.precio, v.porcentajeComision, s.tarifaExtra, v.montoTotal, p.EquipoLocal, p.EquipoVisitante, EL.bandera as banderaEquipoLocal, EV.bandera as banderaEquipoVisitante
                FROM Venta v
                JOIN Entrada e on v.identificador = e.identificadorVenta
                JOIN Partido p on e.identificadorPartido = p.identificador
                JOIN Sector s on e.identificadorEstadio = s.identificadorEstadio and e.identificadorSector = s.identificador
                JOIN Equipo EL on p.EquipoLocal = EL.nombre
                JOIN Equipo EV on p.EquipoVisitante = EV.nombre
                WHERE mailUsuarioComprado = @mail;
            """;

            command.Parameters.AddWithValue("@mail", tokenMail);
            await using var reader = await command.ExecuteReaderAsync();

            var ventas = new List<object>();

            while (await reader.ReadAsync())
            {
                ventas.Add(new
                {
                    Identificador = reader.GetInt32("identificador"),
                    Fecha = reader.GetDateTime("fecha"),
                    Precio = reader.GetInt32("precio"),
                    PorcentajeComision = reader.GetInt32("porcentajeComision"),
                    TarifaExtra = reader.GetInt32("tarifaExtra"),
                    MontoTotal = reader.GetInt32("montoTotal"),
                    EquipoLocal = reader.GetString("EquipoLocal"),
                    EquipoVisitante = reader.GetString("EquipoVisitante"),
                    BanderaEquipoLocal = reader.GetString("banderaEquipoLocal"),
                    BanderaEquipoVisitante = reader.GetString("banderaEquipoVisitante")
                });
            }

            return Results.Ok(ventas);
        }).RequireAuthorization();
    }
}