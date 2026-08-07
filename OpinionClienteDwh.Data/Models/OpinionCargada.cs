namespace OpinionClienteDwh.Data.Models;

public sealed record OpinionCargada
{
    public required int IdOpinion { get; init; }
    public required string IdOpinionOrigen { get; init; }
    public required int IdFuente { get; init; }
}