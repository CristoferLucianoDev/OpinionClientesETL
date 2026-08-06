namespace OpinionClienteDwh.Data.Interfaces;

public interface IExtractor<T>
{
    Task<IReadOnlyList<T>> ExtraerAsync(CancellationToken cancellationToken);
}