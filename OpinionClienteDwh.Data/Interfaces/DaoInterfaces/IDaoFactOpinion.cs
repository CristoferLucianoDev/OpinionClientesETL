using OpinionClienteDwh.Data.Models;
using System.Data;

namespace OpinionClienteDwh.Data.Interfaces.DaoInterfaces;

public interface IDaoFactOpinion
{
    Task LimpiarStagingAsync();
    Task CargarStagingAsync(DataTable staging);
    Task<List<OpinionCargada>> CargarDesdeStagingAsync();
}