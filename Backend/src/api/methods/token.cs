namespace api.Methods;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public static class Token
{
    public static IResult SetToken(IConfiguration config, HttpResponse response, string mail, string typeUser)
    {
        var key = config["Jwt:Key"];

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
                new Claim(ClaimTypes.Name, mail),
                new Claim(ClaimTypes.Email, mail),
                new Claim(ClaimTypes.Role, typeUser)
            }),
            Expires = DateTime.UtcNow.AddHours(3),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(byteKey),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDes);
        var jwt = tokenHandler.WriteToken(token);

        response.Cookies.Append("access_token", jwt, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddHours(3)
        });

        return Results.Ok();
    }

    public static string? GetMailUser(HttpContext context)
    {
        var mail = context.User.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrWhiteSpace(mail))
        {
            return null;
        }

        return mail;
    }

    public static string? GetRoleUser(HttpContext context)
    {
        var role = context.User.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrWhiteSpace(role))
        {
            return null;
        }

        return role;
    }

    public static void ClearToken(HttpResponse response)
    {
        response.Cookies.Delete("access_token", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None
        });
    }
}