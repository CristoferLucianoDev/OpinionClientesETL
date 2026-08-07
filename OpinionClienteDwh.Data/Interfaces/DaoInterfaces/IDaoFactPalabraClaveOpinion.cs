using System.Data;

namespace OpinionClienteDwh.Data.Interfaces.DaoInterfaces;

public interface IDaoFactPalabraClaveOpinion
{
    Task<Dictionary<string, int>> ResolverPalabrasClaveAsync(IReadOnlyCollection<string> palabras);
    Task LimpiarStagingAsync();
    Task CargarStagingAsync(DataTable staging);
    Task CargarDesdeStagingAsync();
}