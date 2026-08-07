namespace OpinionClienteDwh.Data.Models;

public sealed record PalabraClaveResuelta
{
    public required int IdPalabraClave { get; init; }
    public required string Palabra { get; init; }
}