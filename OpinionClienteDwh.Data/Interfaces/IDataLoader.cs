namespace OpinionClienteDwh.Data.Interfaces;

public interface IDataLoader
{
    Task SaveAsync(Services.ExtractResult resultado, CancellationToken cancellationToken);
}