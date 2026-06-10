namespace api.DTOs;

public sealed record UserRequest(
    string MailPerfil
);

public sealed record UserUpdateRequest(
    string EstadoVerificado
);