namespace api.DTOs;

public sealed record PartidoRequest(
    string Fase,
    string EquipoLocal,
    string EquipoVisitante,
    int IdentificadorEstadio,
    DateTime FechaHora,
    int Precio
);

public sealed record PartidoUpdateRequest(
    string Fase,
    string EquipoLocal,
    string EquipoVisitante,
    int IdentificadorEstadio,
    DateTime FechaHora,
    int Precio
);
