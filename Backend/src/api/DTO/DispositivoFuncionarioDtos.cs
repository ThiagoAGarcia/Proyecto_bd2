namespace api.DTO;

public sealed record DispositivoFuncionarioRequest(
    string MailFuncionario,
    int IdentificadorDispositivo
);