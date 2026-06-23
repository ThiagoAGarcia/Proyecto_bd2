using api.Methods;
using MySqlConnector;
using api.DTOs;

namespace api.Endpoints;

public static class PartidoEndpoints
{
    public static void MapPartidoEndpoints(this WebApplication app)
    {
        app.MapPost("/partido", async (PartidoRequest request, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            if (request.EquipoLocal == request.EquipoVisitante)
            {
                return Results.BadRequest(new
                {
                    message = "El equipo local y el equipo visitante no pueden ser el mismo."
                });
            }

            if (request.FechaHora < DateTime.UtcNow)
            {
                return Results.BadRequest(new
                {
                    message = "La fecha y hora del partido no pueden ser en el pasado."
                });
            }

            if (request.Precio < 0)
            {
                return Results.BadRequest(new
                {
                    message = "El precio del partido no puede ser negativo."
                });
            }

            await using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO `partido` (`fase`, `EquipoLocal`, `EquipoVisitante`, `identificadorEstadio`, `fechaHora`, `precio`)
                VALUES (@fase, @equipoLocal, @equipoVisitante, @identificadorEstadio, @fechaHora, @precio);
                """;

            command.Parameters.AddWithValue("@fase", request.Fase);
            command.Parameters.AddWithValue("@equipoLocal", request.EquipoLocal);
            command.Parameters.AddWithValue("@equipoVisitante", request.EquipoVisitante);
            command.Parameters.AddWithValue("@identificadorEstadio", request.IdentificadorEstadio);
            command.Parameters.AddWithValue("@fechaHora", request.FechaHora);
            command.Parameters.AddWithValue("@precio", request.Precio);

            await command.ExecuteNonQueryAsync();

            return Results.Created($"/partido", new
            {
                success = true,
                Fase = request.Fase,
                EquipoLocal = request.EquipoLocal,
                EquipoVisitante = request.EquipoVisitante,
                IdentificadorEstadio = request.IdentificadorEstadio,
                FechaHora = request.FechaHora,
                Precio = request.Precio,
            });
        }).RequireAuthorization("SoloAdministrador");

        app.MapGet("/partido/{identificador}", async (int identificador, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT p.identificador,
                    p.fase,
                    p.EquipoLocal,
                    p.EquipoVisitante,
                    p.identificadorEstadio,
                    p.fechaHora,
                    p.precio,
                    el.Nombre AS NombreEstadio,
                    el.Imagen AS ImagenEstadio,
                    el.DireccionLocalidad AS DireccionLocalidadEstadio,
                    el.DireccionCalle AS DireccionCalleEstadio,
                    el.NombrePais AS NombrePaisEstadio
                FROM partido p
                JOIN estadio el ON p.identificadorEstadio = el.identificador
                WHERE p.identificador = @identificador
                LIMIT 1;
                """;

            command.Parameters.AddWithValue("@identificador", identificador);

            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return Results.NotFound();
            }

            return Results.Ok(new
            {
                Identificador = reader.GetInt32("identificador"),
                Fase = reader.GetString("fase"),
                EquipoLocal = reader.GetString("EquipoLocal"),
                EquipoVisitante = reader.GetString("EquipoVisitante"),
                IdentificadorEstadio = reader.GetInt32("identificadorEstadio"),
                FechaHora = reader.GetDateTime("fechaHora"),
                Precio = reader.GetInt32("precio"),
                NombreEstadio = reader.GetString("NombreEstadio"),
                ImagenEstadio = reader.GetString("ImagenEstadio"),
                DireccionLocalidadEstadio = reader.GetString("DireccionLocalidadEstadio"),
                DireccionCalleEstadio = reader.GetString("DireccionCalleEstadio"),
                NombrePaisEstadio = reader.GetString("NombrePaisEstadio")
            });
        }).RequireAuthorization();

        app.MapDelete("/deletePartido/{identificador}", async (int identificador, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using (var checkCommand = connection.CreateCommand())
            {
                checkCommand.CommandText = """
                    SELECT COUNT(*)
                    FROM Entrada
                    WHERE identificadorPartido = @identificador;
                """;

                checkCommand.Parameters.AddWithValue("@identificador", identificador);

                var cantidadEntradas = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());

                if (cantidadEntradas > 0)
                {
                    return Results.BadRequest(new
                    {
                        message = "No es posible eliminar el partido porque existen entradas asociadas."
                    });
                }
            }

            await using (var deleteEsAsignado = connection.CreateCommand())
            {
                deleteEsAsignado.CommandText = """
                    DELETE FROM EsAsignado
                    WHERE identificadorPartido = @identificador;
                """;

                deleteEsAsignado.Parameters.AddWithValue("@identificador", identificador);

                await deleteEsAsignado.ExecuteNonQueryAsync();
            }

            await using (var deleteHabilita = connection.CreateCommand())
            {
                deleteHabilita.CommandText = """
                    DELETE FROM Habilita
                    WHERE identificadorPartido = @identificador;
                """;

                deleteHabilita.Parameters.AddWithValue("@identificador", identificador);

                await deleteHabilita.ExecuteNonQueryAsync();
            }

            await using (var deleteGestiona = connection.CreateCommand())
            {
                deleteGestiona.CommandText = """
                    DELETE FROM Gestiona
                    WHERE identificadorPartido = @identificador;
                """;

                deleteGestiona.Parameters.AddWithValue("@identificador", identificador);

                await deleteGestiona.ExecuteNonQueryAsync();
            }

            await using (var deleteCommand = connection.CreateCommand())
            {
                deleteCommand.CommandText = """
                    DELETE FROM Partido
                    WHERE identificador = @identificador;
                """;

                deleteCommand.Parameters.AddWithValue("@identificador", identificador);

                var rowsAffected = await deleteCommand.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    return Results.NotFound();
                }
            }

            return Results.NoContent();
        }).RequireAuthorization("SoloAdministrador");

        app.MapPut("/partidos/{identificador}", async (int identificador, PartidoUpdateRequest request, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            if (request.EquipoLocal == request.EquipoVisitante)
            {
                return Results.BadRequest(new
                {
                    message = "El equipo local y el equipo visitante no pueden ser el mismo."
                });
            }

            if (request.FechaHora < DateTime.UtcNow)
            {
                return Results.BadRequest(new
                {
                    message = "La fecha y hora del partido no pueden ser en el pasado."
                });
            }

            if (request.Precio < 0)
            {
                return Results.BadRequest(new
                {
                    message = "El precio del partido no puede ser negativo."
                });
            }

            await using var command = connection.CreateCommand();
            command.CommandText = """
                UPDATE `partido`
                SET `fase` = @fase,
                    `EquipoLocal` = @equipoLocal,
                    `EquipoVisitante` = @equipoVisitante,
                    `identificadorEstadio` = @identificadorEstadio,
                    `fechaHora` = @fechaHora,
                    `precio` = @precio
                WHERE `identificador` = @identificador;
                """;

            command.Parameters.AddWithValue("@identificador", identificador);
            command.Parameters.AddWithValue("@fase", request.Fase);
            command.Parameters.AddWithValue("@equipoLocal", request.EquipoLocal);
            command.Parameters.AddWithValue("@equipoVisitante", request.EquipoVisitante);
            command.Parameters.AddWithValue("@identificadorEstadio", request.IdentificadorEstadio);
            command.Parameters.AddWithValue("@fechaHora", request.FechaHora);
            command.Parameters.AddWithValue("@precio", request.Precio);

            var rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected == 0)
            {
                return Results.NotFound();
            }

            return Results.Ok(new
            {
                success = true,
                Identificador = identificador,
                Fase = request.Fase,
                EquipoLocal = request.EquipoLocal,
                EquipoVisitante = request.EquipoVisitante,
                IdentificadorEstadio = request.IdentificadorEstadio,
                FechaHora = request.FechaHora,
                Precio = request.Precio
            });

        }).RequireAuthorization("SoloAdministrador");

        app.MapGet("/partidos", async (IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT p.identificador,
                    p.fase,
                    p.EquipoLocal,
                    p.EquipoVisitante,
                    p.identificadorEstadio,
                    p.fechaHora,
                    p.precio,

                    el.nombre AS NombreEstadio,
                    el.imagen AS ImagenEstadio,
                    el.direccionLocalidad AS DireccionLocalidadEstadio,
                    el.direccionCalle AS DireccionCalleEstadio,
                    el.nombrePais AS NombrePaisEstadio,

                    eql.bandera AS BanderaEquipoLocal,
                    eqv.bandera AS BanderaEquipoVisitante
                FROM partido p
                JOIN estadio el ON p.identificadorEstadio = el.identificador
                JOIN equipo eql ON p.EquipoLocal = eql.nombre
                JOIN equipo eqv ON p.EquipoVisitante = eqv.nombre WHERE p.fechaHora >= CURRENT_TIMESTAMP()
                ORDER BY p.fechaHora ASC;
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
                    IdentificadorEstadio = reader.GetInt32("identificadorEstadio"),
                    FechaHora = reader.GetDateTime("fechaHora"),
                    NombreEstadio = reader.GetString("NombreEstadio"),
                    ImagenEstadio = reader["ImagenEstadio"] as string,
                    DireccionLocalidadEstadio = reader.GetString("DireccionLocalidadEstadio"),
                    DireccionCalleEstadio = reader.GetString("DireccionCalleEstadio"),
                    NombrePaisEstadio = reader.GetString("NombrePaisEstadio"),
                    Precio = reader.GetInt32("precio"),
                    BanderaEquipoLocal = reader["BanderaEquipoLocal"] as string,
                    BanderaEquipoVisitante = reader["BanderaEquipoVisitante"] as string
                });
            }

            return Results.Ok(partidos);
        }).RequireAuthorization("SoloUsuario");

        app.MapGet("/partidoFase/{fase}", async (string fase, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT p.identificador,
                    p.fase,
                    p.EquipoLocal,
                    p.EquipoVisitante,
                    p.identificadorEstadio,
                    p.fechaHora,
                    el.Nombre AS NombreEstadio,
                    el.Imagen AS ImagenEstadio,
                    el.DireccionLocalidad AS DireccionLocalidadEstadio,
                    el.DireccionCalle AS DireccionCalleEstadio,
                    el.NombrePais AS NombrePaisEstadio
                FROM partido p
                JOIN estadio el ON p.identificadorEstadio = el.identificador
                WHERE p.fase = @fase;
                """;

            command.Parameters.AddWithValue("@fase", fase);

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
                    IdentificadorEstadio = reader.GetInt32("identificadorEstadio"),
                    FechaHora = reader.GetDateTime("fechaHora"),
                    NombreEstadio = reader.GetString("NombreEstadio"),
                    ImagenEstadio = reader.GetString("ImagenEstadio"),
                    DireccionLocalidadEstadio = reader.GetString("DireccionLocalidadEstadio"),
                    DireccionCalleEstadio = reader.GetString("DireccionCalleEstadio"),
                    NombrePaisEstadio = reader.GetString("NombrePaisEstadio")
                });
            }

            return Results.Ok(partidos);
        }).RequireAuthorization();

        app.MapGet("/partidoEquipo/{equipo}", async (string equipo, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT p.identificador,
                    p.fase,
                    p.EquipoLocal,
                    p.EquipoVisitante,
                    p.identificadorEstadio,
                    p.fechaHora,
                    el.Nombre AS NombreEstadio,
                    el.Imagen AS ImagenEstadio,
                    el.DireccionLocalidad AS DireccionLocalidadEstadio,
                    el.DireccionCalle AS DireccionCalleEstadio,
                    el.NombrePais AS NombrePaisEstadio
                FROM partido p
                JOIN estadio el ON p.identificadorEstadio = el.identificador
                WHERE p.EquipoLocal = @equipo
                OR p.EquipoVisitante = @equipo;
                """;

            command.Parameters.AddWithValue("@equipo", equipo);

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
                    IdentificadorEstadio = reader.GetInt32("identificadorEstadio"),
                    FechaHora = reader.GetDateTime("fechaHora"),
                    NombreEstadio = reader.GetString("NombreEstadio"),
                    ImagenEstadio = reader.GetString("ImagenEstadio"),
                    DireccionLocalidadEstadio = reader.GetString("DireccionLocalidadEstadio"),
                    DireccionCalleEstadio = reader.GetString("DireccionCalleEstadio"),
                    NombrePaisEstadio = reader.GetString("NombrePaisEstadio")
                });
            }

            return Results.Ok(partidos);

        }).RequireAuthorization();

        app.MapGet("/allMyPartidos", async (IConfiguration config, HttpContext context) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            var tokenMail = Normalizar.NormalizarMethod(Token.GetMailUser(context));

            await using var paisMailCommand = connection.CreateCommand();

            paisMailCommand.CommandText = """
                SELECT nombrePais
                FROM Administrador
                WHERE mailPerfil = @mailAdministrador;
            """;

            paisMailCommand.Parameters.AddWithValue("@mailAdministrador", tokenMail);

            var tokenMailPais = (await paisMailCommand.ExecuteScalarAsync()) as string;

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT p.identificador,
                    p.fase,
                    p.EquipoLocal,
                    p.EquipoVisitante,
                    p.identificadorEstadio,
                    p.fechaHora,
                    p.precio,

                    el.nombre AS NombreEstadio,
                    el.imagen AS ImagenEstadio,
                    el.direccionLocalidad AS DireccionLocalidadEstadio,
                    el.direccionCalle AS DireccionCalleEstadio,
                    el.nombrePais AS NombrePaisEstadio,

                    eql.bandera AS BanderaEquipoLocal,
                    eqv.bandera AS BanderaEquipoVisitante
                FROM partido p
                JOIN estadio el ON p.identificadorEstadio = el.identificador
                JOIN equipo eql ON p.EquipoLocal = eql.nombre
                JOIN equipo eqv ON p.EquipoVisitante = eqv.nombre
                WHERE el.nombrePais = @nombrePais;
            """;

            command.Parameters.AddWithValue("@nombrePais", tokenMailPais);

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
                    IdentificadorEstadio = reader.GetInt32("identificadorEstadio"),
                    FechaHora = reader.GetDateTime("fechaHora"),
                    NombreEstadio = reader.GetString("NombreEstadio"),
                    ImagenEstadio = reader["ImagenEstadio"] as string,
                    DireccionLocalidadEstadio = reader.GetString("DireccionLocalidadEstadio"),
                    DireccionCalleEstadio = reader.GetString("DireccionCalleEstadio"),
                    NombrePaisEstadio = reader.GetString("NombrePaisEstadio"),
                    Precio = reader.GetInt32("precio"),
                    BanderaEquipoLocal = reader["BanderaEquipoLocal"] as string,
                    BanderaEquipoVisitante = reader["BanderaEquipoVisitante"] as string
                });
            }

            return Results.Ok(partidos);
        }).RequireAuthorization("SoloAdministrador");

        app.MapGet("/allMyPartidosSectores", async (IConfiguration config, HttpContext context) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            var tokenMail = Normalizar.NormalizarMethod(Token.GetMailUser(context));

            await using var paisMailCommand = connection.CreateCommand();

            paisMailCommand.CommandText = """
                SELECT nombrePais
                FROM Administrador
                WHERE mailPerfil = @mailAdministrador;
            """;

            paisMailCommand.Parameters.AddWithValue("@mailAdministrador", tokenMail);

            var tokenMailPais = (await paisMailCommand.ExecuteScalarAsync()) as string;

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT p.identificador AS identificadorPartido,
                    p.fase,
                    p.EquipoLocal,
                    p.EquipoVisitante,
                    p.fechaHora,

                    s.identificador AS identificadorSector,
                    s.nombre AS nombreSector,
                    s.capMax,
                    s.tarifaExtra,

                    d.identificador AS identificadorDispositivo,
                    f.mailPerfil AS mailFuncionario,
                    f.numeroLegajo,

                    ea.fecha AS fechaAsignacion
                FROM Partido p
                INNER JOIN Habilita h ON p.identificador = h.identificadorPartido
                INNER JOIN Sector s ON h.identificadorEstadio = s.identificadorEstadio AND h.identificadorSector = s.identificador
                INNER JOIN Estadio e ON p.identificadorEstadio = e.identificador
                LEFT JOIN EsAsignado ea ON h.identificadorPartido = ea.identificadorPartido AND h.identificadorEstadio = ea.identificadorEstadio AND h.identificadorSector = ea.identificadorSector
                LEFT JOIN Dispositivo d ON ea.identificadorDispositivo = d.identificador
                LEFT JOIN Funcionario f ON d.mailFuncionario = f.mailPerfil
                WHERE e.nombrePais = @nombrePais
                ORDER BY p.identificador, s.nombre;
            """;

            command.Parameters.AddWithValue("@nombrePais", tokenMailPais);

            await using var reader = await command.ExecuteReaderAsync();

            var partidos = new List<object>();

            while (await reader.ReadAsync())
            {
                partidos.Add(new
                {
                    success = true,
                    IdentificadorPartido = reader.GetInt32("identificadorPartido"),
                    Fase = reader.GetString("fase"),
                    EquipoLocal = reader.GetString("EquipoLocal"),
                    EquipoVisitante = reader.GetString("EquipoVisitante"),
                    FechaHora = reader.GetDateTime("fechaHora"),
                    IdentificadorSector = reader.GetInt32("identificadorSector"),
                    NombreSector = reader.GetString("nombreSector"),
                    CapMax = reader.GetInt32("capMax"),
                    TarifaExtra = reader.GetInt32("tarifaExtra"),
                    IdentificadorDispositivo = reader.IsDBNull(reader.GetOrdinal("identificadorDispositivo")) ? (int?)null : reader.GetInt32("identificadorDispositivo"),
                    MailFuncionario = reader.IsDBNull(reader.GetOrdinal("mailFuncionario")) ? null : reader.GetString("mailFuncionario"),
                    NumeroLegajo = reader.IsDBNull(reader.GetOrdinal("numeroLegajo")) ? (int?)null : reader.GetInt32("numeroLegajo"),
                    FechaAsignacion = reader.IsDBNull(reader.GetOrdinal("fechaAsignacion")) ? (DateTime?)null : reader.GetDateTime("fechaAsignacion")
                });
            }

            return Results.Ok(partidos);
        }).RequireAuthorization("SoloAdministrador");
    }
}