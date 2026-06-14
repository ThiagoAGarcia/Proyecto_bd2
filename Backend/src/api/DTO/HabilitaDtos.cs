namespace api.DTOs;

public sealed record HabilitaRequest(
    int IdentificadorPartido,
    int IdentificadorSector,
    int IdentificadorEstadio
);

