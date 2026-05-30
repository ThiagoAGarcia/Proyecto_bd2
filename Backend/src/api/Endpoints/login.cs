using api.Data;
using api.Models;
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
        // POST /loginCheck
        // Chequear login en la base de datos.
        app.MapPost("/loginCheck", async (Login login, AppDbContext db, IConfiguration config) =>
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
            } else if (await db.Administradors.FindAsync(login.MailPerfil) != null)
            {
                typeUser = "Administrador";
            } else if (await db.Funcionarios.FindAsync(login.MailPerfil) != null)
            {
                typeUser = "Funcionario";
            } else
            {
                return Results.Problem("No se pudo determinar el tipo de usuario");
            }

            var key = config["Jwt:Key"];
            Console.WriteLine($"JWT KEY CONFIG: {key}");
            if (string.IsNullOrWhiteSpace(key))
            {
                return Results.Problem("No está configurada la clave JWT");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var byteKey = Encoding.UTF8.GetBytes(key);

            var tokenDes = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, existingLogin.MailPerfil),
                    new Claim(ClaimTypes.Email, existingLogin.MailPerfil)
                }),
                Expires = DateTime.UtcNow.AddMonths(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(byteKey),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDes);
            var jwt = tokenHandler.WriteToken(token);
            
            return Results.Ok(new
            {
                message = "Login correcto",
                token = jwt,
                role = typeUser
            });
        });

        // POST /login
        // Crea un nuevo login en la base de datos.
        app.MapPost("/login", async (Login login, AppDbContext db) =>
        {
            var password = BCrypt.Net.BCrypt.HashPassword(login.Password); 

            Login loginHashed = new(){ MailPerfil = login.MailPerfil, Password = password };
            db.Logins.Add(loginHashed);
            await db.SaveChangesAsync();

            return Results.Created($"/login/{login.MailPerfil}", login);
        });

        app.MapDelete("/login/{mail}", async (string mail, AppDbContext db) =>
        {
            var login = await db.Logins.FindAsync(mail);

            if (login is null)
            {
                return Results.NotFound();
            }

            db.Logins.Remove(login);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}
