using Microsoft.Extensions.Configuration;
using OpinionClienteDwh.Data.Interfaces.DaoInterfaces;
using OpinionClienteDwh.Data.Load.Base;
using OpinionClienteDwh.Data.Models;
using System.Data;

namespace OpinionClienteDwh.Data.Load.Daos;

public sealed class DaoFactOpinion(IConfiguration configuration)
    : OpinionOlapConnection(configuration), IDaoFactOpinion
{
    public Task LimpiarStagingAsync()
        => ExecuteNonQueryAsync("dbo.SP_LimpiarStagingFactOpinion", command => { });

    public Task CargarStagingAsync(DataTable staging)
        => BulkInsertAsync(
            EsquemasStaging.FactOpinion.NombreTabla,
            staging,
            EsquemasStaging.FactOpinion.Mapeos);

    public Task<List<OpinionCargada>> CargarDesdeStagingAsync()
    {
        return ExecuteReaderAsync(
            "dbo.SP_CargarFactOpinionDesdeStaging",
            command => { },
            reader => new OpinionCargada
            {
                IdOpinion = reader.GetInt32(reader.GetOrdinal("IdOpinion")),
                IdOpinionOrigen = reader.GetString(reader.GetOrdinal("IdOpinionOrigen")),
                IdFuente = reader.GetInt32(reader.GetOrdinal("IdFuente"))
            });
    }
}