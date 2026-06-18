namespace api.DTO;

public sealed record DispositivoRequest(
    int Identificador
);

public sealed record DispositivoUpdateRequest(
    int IdentificadorNuevo
);