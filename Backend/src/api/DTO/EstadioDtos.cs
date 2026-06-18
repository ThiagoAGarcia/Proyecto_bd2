
namespace api.DTO;

public sealed record EstadioRequest(
    string Nombre,
    string Imagen,
    string NombrePais,
    string DireccionLocalidad,
    string DireccionCalle,
    int DireccionNumero,
    int DireccionCodigoPostal
);
public sealed record EstadioUpdateRequest(
    string Nombre,
    string Imagen,
    string NombrePais,
    string DireccionLocalidad,
    string DireccionCalle,
    int DireccionNumero,
    int DireccionCodigoPostal
);


