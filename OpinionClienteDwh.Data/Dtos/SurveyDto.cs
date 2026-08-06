namespace OpinionClienteDwh.Data.Dtos;

public sealed record SurveyDto
{
    public required string IdOriginal { get; init; }
    public required int IdCliente { get; init; }
    public required int IdProducto { get; init; }
    public required DateTime Fecha { get; init; }
    public required string Comentario { get; init; }
    public required decimal PuntajeSatisfaccion { get; init; }
    public required string Clasificacion { get; init; }
}