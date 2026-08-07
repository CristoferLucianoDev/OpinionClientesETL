using OpinionClienteDwh.Data.Models;

namespace OpinionClienteDwh.Data.Interfaces;

public interface ILectorOpinionesConsolidadas
{
    Task<IReadOnlyList<OpinionConsolidada>> ObtenerAsync(CancellationToken cancellationToken);
}