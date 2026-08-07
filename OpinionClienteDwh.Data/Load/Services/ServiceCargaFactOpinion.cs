using Microsoft.Extensions.Logging;
using OpinionClienteDwh.Data.Interfaces;
using OpinionClienteDwh.Data.Interfaces.DaoInterfaces;
using OpinionClienteDwh.Data.Models;
using System.Data;

namespace OpinionClienteDwh.Data.Load.Services;

public sealed class ServiceCargaFactOpinion(
    IDaoFactOpinion daoFactOpinion,
    IDimensionKeyCache dimensionKeyCache,
    ILogger<ServiceCargaFactOpinion> logger) : IServiceCargaFactOpinion
{
    private readonly IDaoFactOpinion _daoFactOpinion = daoFactOpinion;
    private readonly IDimensionKeyCache _dimensionKeyCache = dimensionKeyCache;
    private readonly ILogger<ServiceCargaFactOpinion> _logger = logger;

    public async Task<List<OpinionCargada>> EjecutarAsync(
        IReadOnlyCollection<OpinionConsolidada> opiniones,
        IReadOnlySet<int> idsClientesValidos,
        IReadOnlySet<int> idsProductosValidos)
    {
        var staging = await ConstruirStagingAsync(opiniones, idsClientesValidos, idsProductosValidos);

        await _daoFactOpinion.LimpiarStagingAsync();
        await _daoFactOpinion.CargarStagingAsync(staging);

        return await _daoFactOpinion.CargarDesdeStagingAsync();
    }

    private async Task<DataTable> ConstruirStagingAsync(
        IReadOnlyCollection<OpinionConsolidada> opiniones,
        IReadOnlySet<int> idsClientesValidos,
        IReadOnlySet<int> idsProductosValidos)
    {
        var staging = EsquemasStaging.FactOpinion.CrearTabla();

        foreach (var opinion in opiniones)
        {
            if (!idsClientesValidos.Contains(opinion.IdCliente))
            {
                _logger.LogWarning(
                    "Opinion {IdOpinionOrigen} ({Origen}) descartada: IdCliente {IdCliente} no existe en Cliente.",
                    opinion.IdOpinionOrigen, opinion.Origen, opinion.IdCliente);
                continue;
            }

            if (!idsProductosValidos.Contains(opinion.IdProducto))
            {
                _logger.LogWarning(
                    "Opinion {IdOpinionOrigen} ({Origen}) descartada: IdProducto {IdProducto} no existe en Producto.",
                    opinion.IdOpinionOrigen, opinion.Origen, opinion.IdProducto);
                continue;
            }

            var idFuente = await _dimensionKeyCache.GetIdFuentePorOrigenAsync(opinion.Origen);
            var idClasificacion = await _dimensionKeyCache.GetIdClasificacionAsync(opinion.Clasificacion);
            var idFecha = _dimensionKeyCache.GetIdFecha(opinion.Fecha);

            var fila = staging.NewRow();
            fila["IdFecha"] = idFecha;
            fila["IdCliente"] = opinion.IdCliente;
            fila["IdProducto"] = opinion.IdProducto;
            fila["IdFuente"] = idFuente;
            fila["IdClasificacion"] = (object?)idClasificacion ?? DBNull.Value;
            fila["Puntaje"] = (object?)opinion.Puntaje ?? DBNull.Value;
            fila["IdOpinionOrigen"] = opinion.IdOpinionOrigen;

            staging.Rows.Add(fila);
        }

        return staging;
    }
}