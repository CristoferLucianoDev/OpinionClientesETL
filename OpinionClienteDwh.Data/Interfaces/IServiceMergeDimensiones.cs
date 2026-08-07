using OpinionClienteDwh.Data.Models;

namespace OpinionClienteDwh.Data.Interfaces;

public interface IServiceMergeDimensiones
{
    Task<DimensionesValidas> EjecutarAsync(CancellationToken cancellationToken);
}