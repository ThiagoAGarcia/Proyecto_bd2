using api.Methods;
using MySqlConnector;
using api.DTOs;

namespace api.Endpoints;

public static class EntradaEndpoints
{
    public static void MapEntradaEndpoints (this WebApplication app)
    {
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
    }
}