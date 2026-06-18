namespace api.DTO;

public sealed record EsAsignadoRequest(
    int IdentificadorDispositivo,
    int IdentificadorSector,
    int IdentificadorEstadio,
    int IdentificadorPartido
);

public sealed record EsAsignadoDeleteRequest(
    int IdentificadorDispositivo,
    int IdentificadorSector,
    int IdentificadorEstadio,
    int IdentificadorPartido
);

public sealed record EsAsignadoUpdateRequest(
    int IdentificadorDispositivo,
    int IdentificadorSector,
    int IdentificadorEstadio,
    int IdentificadorPartido
);
