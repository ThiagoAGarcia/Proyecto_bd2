namespace api.DTOs;

public sealed record SectorRequest(
    int Identificador,
    int IdentificadorEstadio,
    string Nombre,
    int CapMax, 
    int TarifaExtra
);

public sealed record SectorUpdateRequest(
    int Identificador,
    int IdentificadorEstadio,
    string Nombre,
    int CapMax, 
    int TarifaExtra
);