namespace api.Methods;

using System.Security.Claims;
using System.Text;
using MySqlConnector;
public static class User
{
    public static async Task<bool> CheckUserVerificado(
    string mail,
    IConfiguration config,
    HttpContext context)
    {
        if (string.IsNullOrWhiteSpace(mail))
        {
            return false;
        }

        var connectionString = config.GetConnectionString("DefaultConnection");

        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = """
        SELECT `estadoVerificado`
        FROM `Usuario`
        WHERE `MailPerfil` = @mail
        LIMIT 1;
        """;

        Console.WriteLine($"Checking user verification for mail: {mail}");


        command.Parameters.AddWithValue("@mail", mail);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return false;
        }

        Console.WriteLine($"User verification status: {reader.GetString("estadoVerificado")}");

        if (reader.GetString("estadoVerificado") != "verificado")
        {
            return false;
        }

        return true;
    }
}