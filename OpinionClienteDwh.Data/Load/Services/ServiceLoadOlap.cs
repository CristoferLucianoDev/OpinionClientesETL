using OpinionClienteDwh.Data.Interfaces;

namespace OpinionClienteDwh.Data.Load.Services;

public sealed class ServiceLoadOlap(
    ILectorOpinionesConsolidadas lectorOpinionesConsolidadas,
    IServiceCargaFactOpinion serviceCargaFactOpinion,
    IServiceMergeDimensiones serviceMergeDimensiones,
    IServiceCargaFactPalabraClaveOpinion serviceCargaFactPalabraClaveOpinion) : IServiceLoadOlap
{
    public async Task EjecutarCargaAsync(CancellationToken cancellationToken)
    {
        var dimensionesValidas = await serviceMergeDimensiones.EjecutarAsync(cancellationToken);

        var opiniones = await lectorOpinionesConsolidadas.ObtenerAsync(cancellationToken);

        var opinionesCargadas = await serviceCargaFactOpinion.EjecutarAsync(
            opiniones,
            dimensionesValidas.IdsClientesValidos,
            dimensionesValidas.IdsProductosValidos);

        await serviceCargaFactPalabraClaveOpinion.EjecutarAsync(opiniones, opinionesCargadas);
    }
}