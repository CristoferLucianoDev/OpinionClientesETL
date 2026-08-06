namespace OpinionClienteDwh.Data.Interfaces.DaoInterfaces;

public interface IDao<T>
{
    Task<IReadOnlyList<T>> GetAsync(CancellationToken cancellationToken);
}