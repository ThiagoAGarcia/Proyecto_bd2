namespace api.DTOs;

public sealed record LoginRequest(string MailPerfil, string Password);

public sealed record LoginRow(string MailPerfil, string Password);