namespace api.DTOs;

public sealed record VentaRequest(
    int PorcentajeComision,
    int MontoTotal,
    List<EntradaRequest> Entradas
);

public sealed record EntradaRequest(
    int IdentificadorPartido,
    int IdentificadorEstadio,
    int IdentificadorSector
);
