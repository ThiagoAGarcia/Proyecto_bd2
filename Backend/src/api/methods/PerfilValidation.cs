namespace api.Methods;

using System.Security.Claims;
using System.Text;
using MySqlConnector;

public static class PerfilValidation
{

    public static string? ValidarMail(string MailPerfil)
    {
        if (MailPerfil == null)
        {
            return "Mail es requerido";

        }
        if (MailPerfil.Contains(" "))
        {
            return "El mail no puede contener espacios";
        }
        if (MailPerfil.Contains("@") == false || MailPerfil.Contains(".") == false)
        {
            return "El mail no es válido";
        }
        return null;
    }

    public static string? ValidarPassword(string Password)
    {
        if (Password == null)
        {
            return "Contraseña es requerida";
        }
        if (Password.Length < 8)
        {
            return "La contraseña debe tener al menos 8 caracteres";
        }
        if (Password.Length > 64)
        {
            return "La contraseña no puede tener más de 64 caracteres";
        }

        if (Password.All(char.IsLetter))
        {
            return "La contraseña debe contener al menos un número o símbolo";
        }
        return null;
    }

}

