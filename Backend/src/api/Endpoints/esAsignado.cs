using api.Methods;
using MySqlConnector;
using api.DTO;

namespace api.Endpoints;

public static class EsAsignadoEndpoints
{
    public static void MapEsAsignadoEndpoints(this WebApplication app)
    {
        app.MapPost("/nuevoAsignado", async (IConfiguration config, EsAsignadoRequest request) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO EsAsignado (mailFuncionario, identificadorSector, identificadorEstadio) VALUES
                (@mailFuncionario, @identificadorSector, @identificadorEstadio);
                """;

            command.Parameters.AddWithValue("@mailFuncionario", request.MailFuncionario);
            command.Parameters.AddWithValue("@identificadorSector", request.IdentificadorSector);
            command.Parameters.AddWithValue("@identificadorEstadio", request.IdentificadorEstadio);

            try
            {
                await command.ExecuteNonQueryAsync();
            }
            catch (MySqlException ex)
            {
                return Results.Conflict(new{
                    success = false
                });
            }

            return Results.Ok(new
            {
                success = true,
                message = "El funcionario ha sido asignado correctamente"
            });
        }).RequireAuthorization("SoloAdministrador");

        app.MapPost("/asignado/borrar", async (IConfiguration config, EsAsignadoDeleteRequest request) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                DELETE FROM `EsAsignado`
                WHERE 
                    `mailFuncionario` = @mailFuncionario AND
                    `identificadorSector` = @identificadorSector AND
                    `identificadorEstadio` = @identificadorEstadio;
                """;

            command.Parameters.AddWithValue("@mailFuncionario", request.MailFuncionario);
            command.Parameters.AddWithValue("@identificadorSector", request.IdentificadorSector);
            command.Parameters.AddWithValue("@identificadorEstadio", request.IdentificadorEstadio);

            try
            {
                await command.ExecuteNonQueryAsync();
            }
            catch (MySqlException ex)
            {
                return Results.Conflict(new
                {
                    success = false
                });
            }

            return Results.Ok(new
            {
                success = true,
                message = "La asignación ha sido eliminada correctamente"
            });
        }).RequireAuthorization("SoloAdministrador");
    }
}