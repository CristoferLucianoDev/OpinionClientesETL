using Microsoft.Extensions.Configuration;
using OpinionClienteDwh.Data.Interfaces.DaoInterfaces;
using OpinionClienteDwh.Data.Load.Base;
using System.Data;

namespace OpinionClienteDwh.Data.Dao;

public sealed class DaoDimensionKey(IConfiguration configuration)
    : OpinionOlapConnection(configuration), IDaoDimensionKey
{
    public async Task<Dictionary<string, int>> GetFuentesAsync()
    {
        var filas = await ExecuteReaderAsync(
            "dbo.SP_ObtenerDimFuenteDatos",
            command => { },
            reader => (
                IdFuente: reader.GetInt32("IdFuente"),
                TipoFuente: reader.GetString("TipoFuente")));

        return filas.ToDictionary(x => x.TipoFuente, x => x.IdFuente, StringComparer.OrdinalIgnoreCase);
    }

    public async Task<Dictionary<string, int>> GetClasificacionesAsync()
    {
        var filas = await ExecuteReaderAsync(
            "dbo.SP_ObtenerDimClasificacion",
            command => { },
            reader => (
                IdClasificacion: reader.GetInt32("IdClasificacion"),
                Descripcion: reader.GetString("Descripcion")));

        return filas.ToDictionary(x => x.Descripcion, x => x.IdClasificacion, StringComparer.OrdinalIgnoreCase);
    }
}