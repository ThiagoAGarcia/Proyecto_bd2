namespace api.DTOs;

public sealed record TelefonosRequest(
    List<TelefonoRequest> Telefonos
);

public sealed record TelefonoRequest(
    string MailPerfil,
    string Telefono
);