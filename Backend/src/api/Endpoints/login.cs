using api.Data;
using api.Models;
using api.Methods;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace api.Endpoints;

public static class LoginEndpoints
{
    public static void MapLoginEndpoints(this WebApplication app)
    {
        // POST /checkLogin
        // Chequear login en la base de datos.
        var checkLoginHandler = async (Login login, AppDbContext db, IConfiguration config, HttpResponse response) =>
        {
            var existingLogin = await db.Logins.FindAsync(login.MailPerfil);

            if (existingLogin is null)
            {
                return Results.NotFound("Login no encontrado");
            }

            bool isCorrect = BCrypt.Net.BCrypt.Verify(login.Password, existingLogin.Password);

            if (!isCorrect)
            {
                return Results.Unauthorized();
            }

            var typeUser = "";

            if (await db.Usuarios.FindAsync(login.MailPerfil) != null)
            {
                typeUser = "Usuario";
            }
            else if (await db.Administradors.FindAsync(login.MailPerfil) != null)
            {
                typeUser = "Administrador";
            }
            else if (await db.Funcionarios.FindAsync(login.MailPerfil) != null)
            {
                typeUser = "Funcionario";
            }
            else
            {
                return Results.Problem("No se pudo determinar el tipo de usuario");
            }

            Token.SetToken(config, response, existingLogin, typeUser);

            return Results.Ok(new
            {
                message = "Login correcto",
                role = typeUser
            });
        };

        app.MapPost("/checkLogin", checkLoginHandler);
        app.MapPost("/loginCheck", checkLoginHandler);

        // POST /login
        // Crea un nuevo login en la base de datos.
        app.MapPost("/login", async (Login login, AppDbContext db) =>
        {
            var password = BCrypt.Net.BCrypt.HashPassword(login.Password);

            Login loginHashed = new() { MailPerfil = login.MailPerfil, Password = password };
            db.Logins.Add(loginHashed);
            await db.SaveChangesAsync();

            return Results.Created($"/login/{login.MailPerfil}", login);
        });
        app.MapPost("/logout", (HttpResponse response) =>
        {
            Token.ClearToken(response);

            return Results.Ok(new
            {
                message = "Sesión cerrada correctamente"
            });
        });

        app.MapDelete("/login/{mail}", async (string mail, AppDbContext db, HttpResponse response, HttpContext context) =>
        {
            var login = await db.Logins.FindAsync(mail);
            if (Token.GetMailUser(context) != mail)
            {
                return Results.Unauthorized();
            }

            if (login is null)
            {
                return Results.NotFound();
            }
            Token.ClearToken(response);
            db.Logins.Remove(login);
            await db.SaveChangesAsync();

            return Results.NoContent();
        }).RequireAuthorization("soloUsuario");
    }
}
