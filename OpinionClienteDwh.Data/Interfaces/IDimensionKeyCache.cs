namespace OpinionClienteDwh.Data.Interfaces;

public interface IDimensionKeyCache
{
    Task<int> GetIdFuenteAsync(string tipoFuente);

    Task<int> GetIdFuentePorOrigenAsync(string origen);

    Task<int?> GetIdClasificacionAsync(string? descripcion);

    int GetIdFecha(DateOnly fecha);
}