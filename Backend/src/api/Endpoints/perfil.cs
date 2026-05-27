using api.Data;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Endpoints;

public static class PerfilEndpoints
{
    public static void MapPerfilEndpoints(this WebApplication app)
    {
        // GET /perfiles
        // Obtiene todos los perfiles guardados en la base de datos.
        app.MapGet("/perfiles", async (AppDbContext db) =>
            await db.Perfils.ToListAsync());

        // GET /perfiles/{mail}
        // Busca y obtiene un perfil específico usando su mail como clave primaria.
        app.MapGet("/perfiles/{mail}", async (string mail, AppDbContext db) =>
        {
            var perfil = await db.Perfils.FindAsync(mail);

            return perfil is null
                ? Results.NotFound()
                : Results.Ok(perfil);
        });

        // POST /perfiles
        // Crea un nuevo perfil en la base de datos.
        app.MapPost("/perfil", async (Perfil perfil, AppDbContext db) =>
        {
            db.Perfils.Add(perfil);
            await db.SaveChangesAsync();

            return Results.Created($"/perfiles/{perfil.Mail}", perfil);
        });

        // PUT /perfiles/{mail}
        // Actualiza los datos de un perfil existente usando su mail como identificador.
        app.MapPut("/perfil/{mail}", async (string mail, Perfil input, AppDbContext db) =>
        {
            var perfil = await db.Perfils.FindAsync(mail);

            if (perfil is null)
            {
                return Results.NotFound();
            }

            perfil.PaisDocumento = input.PaisDocumento;
            perfil.TipoDocumento = input.TipoDocumento;
            perfil.NumeroDocumento = input.NumeroDocumento;
            perfil.DireccionLocalidad = input.DireccionLocalidad;
            perfil.DireccionNumero = input.DireccionNumero;
            perfil.DireccionCodigoPostal = input.DireccionCodigoPostal;

            await db.SaveChangesAsync();

            return Results.NoContent();
        });

        // DELETE /perfiles/{mail}
        // Elimina un perfil existente usando su mail como identificador.
        app.MapDelete("/perfil/{mail}", async (string mail, AppDbContext db) =>
        {
            var perfil = await db.Perfils.FindAsync(mail);

            if (perfil is null)
            {
                return Results.NotFound();
            }

            db.Perfils.Remove(perfil);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}