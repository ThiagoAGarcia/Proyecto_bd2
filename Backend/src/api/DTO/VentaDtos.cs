namespace api.DTOs;

public sealed record VentaRequest(
    int PorcentajeComision,
    int MontoTotal
);
