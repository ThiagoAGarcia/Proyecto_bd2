using api.Methods;
using MySqlConnector;
using api.DTO;

namespace api.Endpoints;

public static class EstadioEndpoints
{
    public static void MapEstadioEndpoints(this WebApplication app)
    {
        app.MapPost("/estadio", async (EstadioRequest request, IConfiguration config, HttpContext context) =>
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

            var NombrePais = Normalizar.NormalizarMethod(tokenMailPais);
            if (NombrePais != "estados unidos" && NombrePais != "canada" && NombrePais != "mexico")
            {
                return Results.BadRequest(new
                {
                    message = "El país debe ser Estados Unidos, Canadá o Mexico"
                });
            }
            await using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO `Estadio` (`Nombre`, `Imagen`, `NombrePais`, `DireccionLocalidad`, `DireccionCalle`, `DireccionNumero`, `DireccionCodigoPostal`)
                VALUES (@nombre, @imagen, @nombrePais, @direccionLocalidad, @direccionCalle, @direccionNumero, @direccionCodigoPostal);
                """;

            command.Parameters.AddWithValue("@nombre", request.Nombre);
            command.Parameters.AddWithValue("@imagen", request.Imagen);
            command.Parameters.AddWithValue("@nombrePais", NombrePais);
            command.Parameters.AddWithValue("@direccionLocalidad", request.DireccionLocalidad);
            command.Parameters.AddWithValue("@direccionCalle", request.DireccionCalle);
            command.Parameters.AddWithValue("@direccionNumero", request.DireccionNumero);
            command.Parameters.AddWithValue("@direccionCodigoPostal", request.DireccionCodigoPostal);

            await command.ExecuteNonQueryAsync();

            return Results.Created($"/estadio", new
            {
                Success = true,
                Nombre = request.Nombre,
                Imagen = request.Imagen,
                NombrePais = NombrePais,
                DireccionLocalidad = request.DireccionLocalidad,
                DireccionCalle = request.DireccionCalle,
                DireccionNumero = request.DireccionNumero,
                DireccionCodigoPostal = request.DireccionCodigoPostal
            });
        }).RequireAuthorization("SoloAdministrador");

        app.MapGet("/estadio/{identificador}", async (int identificador, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT `Identificador`, `Nombre`, `Imagen`, `NombrePais`, `DireccionLocalidad`, `DireccionCalle`, `DireccionNumero`, `DireccionCodigoPostal`
                FROM `estadio`
                WHERE `Identificador` = @identificador
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
                Identificador = reader.GetInt32("Identificador"),
                Nombre = reader.GetString("Nombre"),
                Imagen = reader["Imagen"] as string,
                NombrePais = reader.GetString("NombrePais"),
                DireccionLocalidad = reader.GetString("DireccionLocalidad"),
                DireccionCalle = reader.GetString("DireccionCalle"),
                DireccionNumero = reader.GetInt32("DireccionNumero"),
                DireccionCodigoPostal = reader.GetInt32("DireccionCodigoPostal")
            });
        }).RequireAuthorization();

        app.MapGet("/allEstadio", async (IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT `Identificador`, `Nombre`, `Imagen`, `NombrePais`, `DireccionLocalidad`, `DireccionCalle`, `DireccionNumero`, `DireccionCodigoPostal`
                FROM `estadio`
                """;


            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return Results.NotFound();
            }

            return Results.Ok(new
            {
                Identificador = reader.GetInt32("Identificador"),
                Nombre = reader.GetString("Nombre"),
                Imagen = reader["Imagen"] as string,
                NombrePais = reader.GetString("NombrePais"),
                DireccionLocalidad = reader.GetString("DireccionLocalidad"),
                DireccionCalle = reader.GetString("DireccionCalle"),
                DireccionNumero = reader.GetInt32("DireccionNumero"),
                DireccionCodigoPostal = reader.GetInt32("DireccionCodigoPostal")
            });
        }).RequireAuthorization();

        app.MapGet("/allMyEstadios", async (IConfiguration config, HttpContext context) =>
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
                SELECT e.identificador,
                    e.nombre,
                    e.imagen,
                    e.direccionLocalidad,
                    e.direccionCalle,
                    e.direccionNumero,
                    e.direccionCodigoPostal,
                    e.nombrePais
                FROM estadio e
                WHERE e.nombrePais = @nombrePais;
            """;

            command.Parameters.AddWithValue("@nombrePais", tokenMailPais);

            await using var reader = await command.ExecuteReaderAsync();

            var estadios = new List<object>();

            while (await reader.ReadAsync())
            {
                estadios.Add(new
                {
                    Identificador = reader.GetInt32("identificador"),
                    Nombre = reader.GetString("Nombre"),
                    Imagen = reader["Imagen"] as string,
                    DireccionLocalidad = reader.GetString("DireccionLocalidad"),
                    DireccionCalle = reader.GetString("DireccionCalle"),
                    DireccionNumero = reader.GetInt32("DireccionNumero"),
                    DireccionCodigoPostal = reader.GetInt32("DireccionCodigoPostal"),
                    NombrePais = reader.GetString("NombrePais")
                });
            }

            return Results.Ok(estadios);
        }).RequireAuthorization("SoloAdministrador");

        app.MapDelete("/estadioDelete/{identificador}", async (int identificador, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                DELETE FROM `estadio`
                WHERE `Identificador` = @identificador
                """;

            command.Parameters.AddWithValue("@identificador", identificador);

            var rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected == 0)
            {
                return Results.NotFound();
            }

            return Results.Ok(new
            {
                success = true,
                message = "Estadio eliminado"
            });
        }).RequireAuthorization("SoloAdministrador");

        app.MapPut("/estadioUpdate/{identificador}", async (int identificador, EstadioUpdateRequest request, IConfiguration config, HttpContext context) =>
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

            var NombrePais = Normalizar.NormalizarMethod(tokenMailPais);

            await using var command = connection.CreateCommand();
            command.CommandText = """
                UPDATE `estadio`
                SET `Nombre` = @nombre,
                    `Imagen` = @imagen,
                    `NombrePais` = @nombrePais,
                    `DireccionLocalidad` = @direccionLocalidad,
                    `DireccionCalle` = @direccionCalle,
                    `DireccionNumero` = @direccionNumero,
                    `DireccionCodigoPostal` = @direccionCodigoPostal
                WHERE `Identificador` = @identificador
                """;

            command.Parameters.AddWithValue("@identificador", identificador);
            command.Parameters.AddWithValue("@nombre", request.Nombre);
            command.Parameters.AddWithValue("@imagen", request.Imagen);
            command.Parameters.AddWithValue("@nombrePais", NombrePais);
            command.Parameters.AddWithValue("@direccionLocalidad", request.DireccionLocalidad);
            command.Parameters.AddWithValue("@direccionCalle", request.DireccionCalle);
            command.Parameters.AddWithValue("@direccionNumero", request.DireccionNumero);
            command.Parameters.AddWithValue("@direccionCodigoPostal", request.DireccionCodigoPostal);

            var rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected == 0)
            {
                return Results.NotFound();
            }

            return Results.Ok(new
            {
                success = true,
                message = "Estadio actualizado"
            });
        }).RequireAuthorization("SoloAdministrador");
    }
}