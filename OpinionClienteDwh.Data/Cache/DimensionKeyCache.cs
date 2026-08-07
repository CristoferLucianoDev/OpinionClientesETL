using OpinionClienteDwh.Data.Interfaces;
using OpinionClienteDwh.Data.Interfaces.DaoInterfaces;

namespace OpinionClienteDwh.Data.Cache;

public sealed class DimensionKeyCache(IDaoDimensionKey daoDimensionKey) : IDimensionKeyCache
{
    private static readonly Dictionary<string, string> MapaOrigenATipoFuente = new(StringComparer.OrdinalIgnoreCase)
    {
        ["WebReview"] = "Web",
        ["SocialComment"] = "Red Social",
        ["Survey"] = "CSV"
    };

    private readonly IDaoDimensionKey _daoDimensionKey = daoDimensionKey;

    private Dictionary<string, int>? _fuentes;
    private Dictionary<string, int>? _clasificaciones;

    public async Task<int> GetIdFuenteAsync(string tipoFuente)
    {
        _fuentes ??= await _daoDimensionKey.GetFuentesAsync();

        if (!_fuentes.TryGetValue(tipoFuente, out var idFuente))
            throw new InvalidOperationException($"TipoFuente '{tipoFuente}' no existe en DimFuenteDatos.");

        return idFuente;
    }

    public Task<int> GetIdFuentePorOrigenAsync(string origen)
    {
        if (!MapaOrigenATipoFuente.TryGetValue(origen, out var tipoFuente))
            throw new InvalidOperationException($"Origen '{origen}' no tiene mapeo a TipoFuente definido.");

        return GetIdFuenteAsync(tipoFuente);
    }

    public async Task<int?> GetIdClasificacionAsync(string? descripcion)
    {
        if (string.IsNullOrWhiteSpace(descripcion))
            return null;

        _clasificaciones ??= await _daoDimensionKey.GetClasificacionesAsync();

        return _clasificaciones.TryGetValue(descripcion, out var idClasificacion)
            ? idClasificacion
            : null;
    }

    public int GetIdFecha(DateOnly fecha)
        => fecha.Year * 10000 + fecha.Month * 100 + fecha.Day;
}