
namespace api.DTOs;

public sealed record EstadioRequest(
    string Nombre,
    string Imagen,
    string NombreJurisdiccion,
    string DireccionLocalidad,
    string DireccionCalle,
    int DireccionNumero,
    int DireccionCodigoPostal
);
public sealed record EstadioUpdateRequest(
    string Nombre,
    string Imagen,
    string NombreJurisdiccion,
    string DireccionLocalidad,
    string DireccionCalle,
    int DireccionNumero,
    int DireccionCodigoPostal
);


