namespace OpinionClienteDwh.Data.Models;

public sealed record OpinionConsolidada
{
    public required string Origen { get; init; }
    public required string IdOpinionOrigen { get; init; }
    public required int IdCliente { get; init; }
    public required int IdProducto { get; init; }
    public required DateOnly Fecha { get; init; }
    public required string Comentario { get; init; }
    public string? Clasificacion { get; init; }
    public decimal? Puntaje { get; init; }
}