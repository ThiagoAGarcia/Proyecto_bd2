namespace api.DTOs;

public sealed record TransferenciaRequest(
    string MailUsuarioDestino,
    int IdentificadorEntrada
);