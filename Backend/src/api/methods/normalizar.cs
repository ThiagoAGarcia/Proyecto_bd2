namespace api.Methods;

using System.Security.Claims;
using System.Text;
using MySqlConnector;
public static class Normalizar
{
    public static string NormalizarMethod(string valor)
    {
        return valor.Trim().ToLower();
    }
}