namespace OpinionClienteDwh.Data.Interfaces.DaoInterfaces;

public interface IDataLoader
{
    Task SaveAsync(Services.ExtractResult resultado, CancellationToken cancellationToken);
}