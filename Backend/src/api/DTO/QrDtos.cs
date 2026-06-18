namespace api.DTOs;

public sealed record QrRequest(
    int IdentificadorEntrada,
    int IdentificadorDispositivo
);
public sealed record QrUpdate(
    int IdentificadorEntrada,
    int IdentificadorDispositivo
);
