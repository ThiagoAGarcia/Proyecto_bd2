namespace api.DTO;

public sealed record EsAsignadoRequest(
    string MailFuncionario,
    int IdentificadorSector,
    int IdentificadorEstadio
);

public sealed record EsAsignadoDeleteRequest(
    string MailFuncionario,
    int IdentificadorSector,
    int IdentificadorEstadio
);

public sealed record EsAsignadoUpdateRequest(
    string MailFuncionario,
    int IdentificadorSector,
    int IdentificadorEstadio
);
