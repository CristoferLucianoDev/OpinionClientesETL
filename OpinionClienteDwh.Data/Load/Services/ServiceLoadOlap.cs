using OpinionClienteDwh.Data.Interfaces;
using OpinionClienteDwh.Data.Interfaces.DaoInterfaces;

namespace OpinionClienteDwh.Data.Load.Services;

public sealed class ServiceLoadOlap(
    IDaoFactPalabraClaveOpinion daoFactPalabraClaveOpinion,
    IDaoFactOpinion daoFactOpinion,
    ILectorOpinionesConsolidadas lectorOpinionesConsolidadas,
    IServiceCargaFactOpinion serviceCargaFactOpinion,
    IServiceMergeDimensiones serviceMergeDimensiones,
    IServiceCargaFactPalabraClaveOpinion serviceCargaFactPalabraClaveOpinion) : IServiceLoadOlap
{
    public async Task EjecutarCargaAsync(CancellationToken cancellationToken)
    {
        // Limpieza de hechos: hijo primero, luego padre (por la FK)
        await daoFactPalabraClaveOpinion.LimpiarFactAsync();
        await daoFactOpinion.LimpiarFactAsync();

        var dimensionesValidas = await serviceMergeDimensiones.EjecutarAsync(cancellationToken);

        var opiniones = await lectorOpinionesConsolidadas.ObtenerAsync(cancellationToken);

        var opinionesCargadas = await serviceCargaFactOpinion.EjecutarAsync(
            opiniones,
            dimensionesValidas.IdsClientesValidos,
            dimensionesValidas.IdsProductosValidos);

        await serviceCargaFactPalabraClaveOpinion.EjecutarAsync(opiniones, opinionesCargadas);
    }
}