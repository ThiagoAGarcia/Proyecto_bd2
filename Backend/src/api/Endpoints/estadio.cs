using api.Methods;
using MySqlConnector;
using api.DTOs;

namespace api.Endpoints;

public static class EstadioEndpoints
{
    public static void MapEstadioEndpoints(this WebApplication app)
    {
        app.MapPost("/estadio", async (EstadioRequest request, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO `Estadio` (`Nombre`, `Imagen`, `NombrePais`, `DireccionLocalidad`, `DireccionCalle`, `DireccionNumero`, `DireccionCodigoPostal`)
                VALUES (@nombre, @imagen, @nombrePais, @direccionLocalidad, @direccionCalle, @direccionNumero, @direccionCodigoPostal);
                """;

            command.Parameters.AddWithValue("@nombre", request.Nombre);
            command.Parameters.AddWithValue("@imagen", request.Imagen);
            command.Parameters.AddWithValue("@nombrePais", request.NombrePais);
            command.Parameters.AddWithValue("@direccionLocalidad", request.DireccionLocalidad);
            command.Parameters.AddWithValue("@direccionCalle", request.DireccionCalle);
            command.Parameters.AddWithValue("@direccionNumero", request.DireccionNumero);
            command.Parameters.AddWithValue("@direccionCodigoPostal", request.DireccionCodigoPostal);

            await command.ExecuteNonQueryAsync();

            return Results.Created($"/estadio", new
            {
                Nombre = request.Nombre,
                Imagen = request.Imagen,
                NombrePais = request.NombrePais,
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
                Imagen = reader.GetString("Imagen"),
                NombrePais = reader.GetString("NombrePais"),
                DireccionLocalidad = reader.GetString("DireccionLocalidad"),
                DireccionCalle = reader.GetString("DireccionCalle"),
                DireccionNumero = reader.GetInt32("DireccionNumero"),
                direccionCodigoPostal = reader.GetInt32("DireccionCodigoPostal")
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
                identificador = reader.GetInt32("Identificador"),
                nombre = reader.GetString("Nombre"),
                imagen = reader.GetString("Imagen"),
                nombrePais = reader.GetString("NombrePais"),
                direccionLocalidad = reader.GetString("DireccionLocalidad"),
                direccionCalle = reader.GetString("DireccionCalle"),
                direccionNumero = reader.GetInt32("DireccionNumero"),
                direccionCodigoPostal = reader.GetInt32("DireccionCodigoPostal")
            });
        }).RequireAuthorization();

        app.MapDelete("/estadio/{identificador}", async (int identificador, IConfiguration config) =>
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
                message = "Estadio eliminado"
            });
        }).RequireAuthorization("SoloAdministrador");

        app.MapPut("/estadio/{identificador}", async (int identificador, EstadioUpdateRequest request, IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

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
            command.Parameters.AddWithValue("@nombrePais", request.NombrePais);
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
                message = "Estadio actualizado"
            });
        }).RequireAuthorization("SoloAdministrador");
    }
}