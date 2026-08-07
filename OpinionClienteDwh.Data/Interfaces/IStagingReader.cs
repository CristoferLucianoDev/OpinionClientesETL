namespace OpinionClienteDwh.Data.Interfaces;

public interface IStagingReader
{
    Task<IReadOnlyList<T>> ReadLastAsync<T>(string prefijo, CancellationToken cancellationToken);
}