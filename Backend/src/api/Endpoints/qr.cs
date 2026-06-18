using api.Methods;
using MySqlConnector;
using api.DTOs;

namespace api.Endpoints;

public static class QrEndpoints
{
    public static void MapQrEndpoints(this WebApplication app)
    {
        app.MapPost("/qr", async (QrRequest request, IConfiguration config, HttpContext context) =>
        {
            var getMail = Token.GetMailUser(context);
            if (getMail == null)
            {
                return Results.Unauthorized();
            }

            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT 1
                FROM Entrada
                WHERE mailUsuarioTiene = @GetMail
                LIMIT 1;
                """;

            command.Parameters.AddWithValue("@GetMail", getMail);

            var existeEntrada = await command.ExecuteScalarAsync();

            if (existeEntrada == null)
            {
                return Results.Unauthorized();
            }

            command.Parameters.Clear();

            command.CommandText = """
                SELECT token, fechaVencimiento
                FROM qr
                WHERE identificadorEntrada = @identificadorEntrada
                LIMIT 1;
                """;

            command.Parameters.AddWithValue("@identificadorEntrada", request.IdentificadorEntrada);

            string tokenVerificacion;
            DateTime fechaVencimiento;

            await using var reader = await command.ExecuteReaderAsync();

            bool existeQr = await reader.ReadAsync();

            if (existeQr)
            {
                tokenVerificacion = reader.GetString("token");
                fechaVencimiento = reader.GetDateTime("fechaVencimiento");
            }
            else
            {
                tokenVerificacion = "";
                fechaVencimiento = DateTime.MinValue;
            }

            await reader.CloseAsync();


            if (!existeQr || fechaVencimiento <= DateTime.UtcNow)
            {
                tokenVerificacion = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                    .Replace("/", "")
                    .Replace("+", "")
                    .Replace("=", "");

                fechaVencimiento = DateTime.UtcNow.AddSeconds(1);

                command.Parameters.Clear();

                if (existeQr)
                {
                    command.CommandText = """
                        UPDATE qr
                        SET token = @token,
                            fechaVencimiento = @fechaVencimiento,
                            identificadorDispositivo = @identificadorDispositivo
                        WHERE identificadorEntrada = @identificadorEntrada;
                        """;
                }
                else
                {
                    command.CommandText = """
                        INSERT INTO qr
                        (identificadorEntrada, token, fechaVencimiento, identificadorDispositivo)
                        VALUES
                        (@identificadorEntrada, @token, @fechaVencimiento, @identificadorDispositivo);
                        """;
                }

                command.Parameters.AddWithValue("@identificadorEntrada", request.IdentificadorEntrada);
                command.Parameters.AddWithValue("@token", tokenVerificacion);
                command.Parameters.AddWithValue("@fechaVencimiento", fechaVencimiento);
                command.Parameters.AddWithValue("@identificadorDispositivo", request.IdentificadorDispositivo);

                await command.ExecuteNonQueryAsync();
            }

            return Results.Ok(new
            {
                IdentificadorEntrada = request.IdentificadorEntrada,
                IdentificadorDispositivo = request.IdentificadorDispositivo,
                Token = tokenVerificacion,
                FechaVencimiento = fechaVencimiento
            });
        })
        .RequireAuthorization();

        app.MapPost("/qr/entrada", async (QrUpdate request, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            string token = "";
            DateTime fechaVencimiento = DateTime.MinValue;

            bool existeQr;

            await using (var command = connection.CreateCommand())
            {
                command.CommandText = """
                    SELECT token, fechaVencimiento
                    FROM qr
                    WHERE identificadorEntrada = @id
                    LIMIT 1;
                """;

                command.Parameters.AddWithValue("@id", request.IdentificadorEntrada);

                await using var reader = await command.ExecuteReaderAsync();

                existeQr = await reader.ReadAsync();

                if (existeQr)
                {
                    token = reader.GetString("token");
                    fechaVencimiento = reader.GetDateTime("fechaVencimiento");
                }
            }

            if (!existeQr || fechaVencimiento <= DateTime.UtcNow)
            {
                token = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                    .Replace("/", "")
                    .Replace("+", "")
                    .Replace("=", "");

                fechaVencimiento = DateTime.UtcNow.AddSeconds(30);

                await using var command = connection.CreateCommand();

                command.CommandText = existeQr
                    ? """
                        UPDATE qr
                        SET token = @token,
                            fechaVencimiento = @fecha,
                            identificadorDispositivo = @disp
                        WHERE identificadorEntrada = @id;
                    """
                    : """
                        INSERT INTO qr
                        (identificadorEntrada, token, fechaVencimiento, identificadorDispositivo)
                        VALUES (@id, @token, @fecha, @disp);
                    """;

                command.Parameters.AddWithValue("@id", request.IdentificadorEntrada);
                command.Parameters.AddWithValue("@token", token);
                command.Parameters.AddWithValue("@fecha", fechaVencimiento);
                command.Parameters.AddWithValue("@disp", request.IdentificadorDispositivo);

                var rows = await command.ExecuteNonQueryAsync();


            }

            return Results.Ok(new
            {
                request.IdentificadorEntrada,
                request.IdentificadorDispositivo,
                Token = token,
                FechaVencimiento = fechaVencimiento
            });
        });

        app.MapGet("/qr/token", async (
            string token,
            string mailPerfil,
            IConfiguration config,
            HttpContext context) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            int identificadorEntrada;
            DateTime fechaVencimiento;

            await using (var command = connection.CreateCommand())
            {
                command.CommandText = """
                    SELECT identificadorEntrada, fechaVencimiento
                    FROM qr
                    WHERE token = @token
                    LIMIT 1;
                """;

                command.Parameters.AddWithValue("@token", token);

                await using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    return Results.Ok(new { valido = false });
                }

                identificadorEntrada = reader.GetInt32(0);
                fechaVencimiento = reader.GetDateTime(1);
            }

            await using (var checkUsed = connection.CreateCommand())
            {
                checkUsed.CommandText = """
                SELECT estadoEntrada
                FROM Entrada
                WHERE identificador = @id;
            """;

                checkUsed.Parameters.AddWithValue("@id", identificadorEntrada);

                await using var reader = await checkUsed.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var estado = reader.GetString(0);

                    if (estado == "Registrada")
                    {
                        return Results.Ok(new { valido = false, motivo = "ya registrado" });
                    }
                }
            }

            if (fechaVencimiento <= DateTime.UtcNow)
            {
                return Results.Ok(new { valido = false, motivo = "expirado" });
            }

            await using (var updateCommand = connection.CreateCommand())
            {
                updateCommand.CommandText = """
                    UPDATE Entrada
                    SET estadoEntrada = 'Registrada', codigoQrAceptado = @token, fechaHoraIngreso = CURRENT_TIMESTAMP(), identificadorDispositivo = (
                        SELECT identificadorDispositivo
                        FROM qr
                        WHERE identificadorEntrada = @id
                    ), mailFuncionario = @mail
                    WHERE identificador = @id;
                """;

                updateCommand.Parameters.AddWithValue("@id", identificadorEntrada);
                updateCommand.Parameters.AddWithValue("@token", token);
                updateCommand.Parameters.AddWithValue("@mail", mailPerfil);

                await updateCommand.ExecuteNonQueryAsync();
            }

            return Results.Ok(new { valido = true });
        });
    }
}