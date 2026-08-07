using OpinionClienteDwh.Data.Interfaces;
using OpinionClienteDwh.Data.Interfaces.DaoInterfaces;
using OpinionClienteDwh.Data.Models;
using System.Data;

namespace OpinionClienteDwh.Data.Load.Services;

public sealed class ServiceCargaFactPalabraClaveOpinion(
    IDaoFactPalabraClaveOpinion daoFactPalabraClaveOpinion,
    IDimensionKeyCache dimensionKeyCache,
    ITokenizadorComentarios tokenizadorComentarios) : IServiceCargaFactPalabraClaveOpinion
{
    private readonly IDaoFactPalabraClaveOpinion _daoFactPalabraClaveOpinion = daoFactPalabraClaveOpinion;
    private readonly IDimensionKeyCache _dimensionKeyCache = dimensionKeyCache;
    private readonly ITokenizadorComentarios _tokenizadorComentarios = tokenizadorComentarios;

    public async Task EjecutarAsync(
        IReadOnlyCollection<OpinionConsolidada> opinionesOrigen,
        IReadOnlyCollection<OpinionCargada> opinionesCargadas)
    {
        await _tokenizadorComentarios.InicializarAsync();

        var mapaIdOpinion = opinionesCargadas.ToDictionary(o => (o.IdOpinionOrigen, o.IdFuente));

        var pendientes = new List<(int IdOpinion, int IdFecha, int IdProducto, int? IdClasificacion, Dictionary<string, int> Palabras)>();
        var palabrasDelLote = new HashSet<string>();

        foreach (var opinion in opinionesOrigen)
        {
            var idFuente = await _dimensionKeyCache.GetIdFuentePorOrigenAsync(opinion.Origen);

            if (!mapaIdOpinion.TryGetValue((opinion.IdOpinionOrigen, idFuente), out var opinionCargada))
                continue;

            var palabras = _tokenizadorComentarios.TokenizarComentario(opinion.Comentario);
            if (palabras.Count == 0)
                continue;

            foreach (var palabra in palabras.Keys)
                palabrasDelLote.Add(palabra);

            var idFecha = _dimensionKeyCache.GetIdFecha(opinion.Fecha);
            var idClasificacion = await _dimensionKeyCache.GetIdClasificacionAsync(opinion.Clasificacion);

            pendientes.Add((opinionCargada.IdOpinion, idFecha, opinion.IdProducto, idClasificacion, palabras));
        }

        if (pendientes.Count == 0)
            return;

        var mapaPalabraClave = await _daoFactPalabraClaveOpinion.ResolverPalabrasClaveAsync(palabrasDelLote);

        var staging = ConstruirStaging(pendientes, mapaPalabraClave);

        await _daoFactPalabraClaveOpinion.LimpiarStagingAsync();
        await _daoFactPalabraClaveOpinion.CargarStagingAsync(staging);
        await _daoFactPalabraClaveOpinion.CargarDesdeStagingAsync();
    }

    private static DataTable ConstruirStaging(
        IReadOnlyCollection<(int IdOpinion, int IdFecha, int IdProducto, int? IdClasificacion, Dictionary<string, int> Palabras)> pendientes,
        Dictionary<string, int> mapaPalabraClave)
    {
        var staging = EsquemasStaging.FactPalabraClaveOpinion.CrearTabla();

        foreach (var (idOpinion, idFecha, idProducto, idClasificacion, palabras) in pendientes)
        {
            foreach (var (palabra, frecuencia) in palabras)
            {
                if (!mapaPalabraClave.TryGetValue(palabra, out var idPalabraClave))
                    continue;

                var fila = staging.NewRow();
                fila["IdOpinion"] = idOpinion;
                fila["IdPalabraClave"] = idPalabraClave;
                fila["IdFecha"] = idFecha;
                fila["IdProducto"] = idProducto;
                fila["IdClasificacion"] = (object?)idClasificacion ?? DBNull.Value;
                fila["Frecuencia"] = frecuencia;

                staging.Rows.Add(fila);
            }
        }

        return staging;
    }
}