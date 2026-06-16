namespace api.DTOs;

public sealed record EquipoRequest(
    string Nombre,
    string Bandera
);

public sealed record EquipoUpdateRequest(
    string Bandera
);
