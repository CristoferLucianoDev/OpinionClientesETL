namespace OpinionClienteDwh.Data.Models;

public sealed record DimensionesValidas
{
    public required IReadOnlySet<int> IdsClientesValidos { get; init; }
    public required IReadOnlySet<int> IdsProductosValidos { get; init; }
}