namespace api.DTO;

public sealed record VentaRequest(
    int PorcentajeComision,
    int MontoTotal
);
