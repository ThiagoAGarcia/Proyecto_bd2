namespace api.DTOs;

public sealed record EntradasRequest(
    List<EntradaRequest> Entradas
);

public sealed record EntradaRequest(
    int IdentificadorPartido,
    int IdentificadorEstadio,
    int IdentificadorSector
);