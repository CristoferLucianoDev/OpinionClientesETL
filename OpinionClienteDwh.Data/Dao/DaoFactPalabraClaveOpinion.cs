using Microsoft.Data.SqlClient.Server;
using Microsoft.Extensions.Configuration;
using OpinionClienteDwh.Data.Common;
using OpinionClienteDwh.Data.Interfaces.DaoInterfaces;
using OpinionClienteDwh.Data.Load.Base;
using OpinionClienteDwh.Data.Models;
using System.Data;

namespace OpinionClienteDwh.Data.Load.Daos;

public sealed class DaoFactPalabraClaveOpinion(IConfiguration configuration)
    : OpinionOlapConnection(configuration), IDaoFactPalabraClaveOpinion
{
    private static readonly SqlMetaData[] ColumnasPalabraClaveTvp =
    [
        new SqlMetaData("Palabra", SqlDbType.VarChar, 100)
    ];

    public async Task<Dictionary<string, int>> ResolverPalabrasClaveAsync(IReadOnlyCollection<string> palabras)
    {
        var resultado = await ExecuteReaderAsync(
            "dbo.SP_ResolverPalabrasClave",
            command => AgregarParametroTvp(
                command,
                "@Palabras",
                "dbo.PalabraClaveTVP",
                TvpBuilder.Construir(
                    ColumnasPalabraClaveTvp,
                    palabras,
                    (registro, palabra) => registro.SetString(0, palabra))),
            reader => new PalabraClaveResuelta
            {
                IdPalabraClave = reader.GetInt32(reader.GetOrdinal("IdPalabraClave")),
                Palabra = reader.GetString(reader.GetOrdinal("Palabra"))
            });

        return resultado.ToDictionary(x => x.Palabra, x => x.IdPalabraClave, StringComparer.OrdinalIgnoreCase);
    }

    public Task LimpiarStagingAsync()
        => ExecuteNonQueryAsync("dbo.SP_LimpiarStagingFactPalabraClaveOpinion", command => { });

    public Task CargarStagingAsync(DataTable staging)
        => BulkInsertAsync(
            EsquemasStaging.FactPalabraClaveOpinion.NombreTabla,
            staging,
            EsquemasStaging.FactPalabraClaveOpinion.Mapeos);

    public Task CargarDesdeStagingAsync()
        => ExecuteNonQueryAsync("dbo.SP_CargarFactPalabraClaveOpinionDesdeStaging", command => { });
}