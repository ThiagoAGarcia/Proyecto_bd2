namespace api.DTOs;

public sealed record HabilitaRequest(
    int IdentificadorPartido,
    int IdentificadorSector,
    int IdentificadorEstadio
);

public record UpdateHabilitaRequest(
    int IdentificadorEstadio,
    int IdentificadorPartido,
    List<int> Sectores
);