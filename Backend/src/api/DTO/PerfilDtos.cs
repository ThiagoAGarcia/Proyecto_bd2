namespace api.DTOs;

public sealed record PerfilRequest(
    string Mail,
    string PaisDocumento,
    string TipoDocumento,
    string NumeroDocumento,
    string DireccionLocalidad,
    int DireccionNumero,
    int DireccionCodigoPostal
);

public sealed record PerfilUpdateRequest(
    string PaisDocumento,
    string TipoDocumento,
    string NumeroDocumento,
    string DireccionLocalidad,
    int DireccionNumero,
    int DireccionCodigoPostal
);

public sealed record PerfilResponse(
    string Mail,
    string PaisDocumento,
    string TipoDocumento,
    string NumeroDocumento,
    string DireccionLocalidad,
    int DireccionNumero,
    int DireccionCodigoPostal
);