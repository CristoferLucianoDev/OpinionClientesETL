using OpinionClienteDwh.Data.Models;

namespace OpinionClienteDwh.Data.Interfaces;

public interface IServiceCargaFactOpinion
{
    Task<List<OpinionCargada>> EjecutarAsync(
        IReadOnlyCollection<OpinionConsolidada> opiniones,
        IReadOnlySet<int> idsClientesValidos,
        IReadOnlySet<int> idsProductosValidos);
}