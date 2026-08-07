namespace OpinionClienteDwh.Data.Interfaces;

public interface IServiceLoadOlap
{
    Task EjecutarCargaAsync(CancellationToken cancellationToken);
}