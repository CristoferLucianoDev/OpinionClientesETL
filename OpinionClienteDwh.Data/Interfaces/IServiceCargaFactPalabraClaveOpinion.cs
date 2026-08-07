using OpinionClienteDwh.Data.Models;

namespace OpinionClienteDwh.Data.Interfaces;

public interface IServiceCargaFactPalabraClaveOpinion
{
    Task EjecutarAsync(
        IReadOnlyCollection<OpinionConsolidada> opinionesOrigen,
        IReadOnlyCollection<OpinionCargada> opinionesCargadas);
}