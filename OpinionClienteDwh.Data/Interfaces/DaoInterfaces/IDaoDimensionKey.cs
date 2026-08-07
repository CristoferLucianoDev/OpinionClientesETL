namespace OpinionClienteDwh.Data.Interfaces.DaoInterfaces;

public interface IDaoDimensionKey
{
    Task<Dictionary<string, int>> GetFuentesAsync();

    Task<Dictionary<string, int>> GetClasificacionesAsync();
}