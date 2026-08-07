using Microsoft.Data.SqlClient.Server;

namespace OpinionClienteDwh.Data.Common;

public static class TvpBuilder
{
    public static IEnumerable<SqlDataRecord> Construir<T>(
        SqlMetaData[] columnas,
        IEnumerable<T> elementos,
        Action<SqlDataRecord, T> asignar)
    {
        foreach (var elemento in elementos)
        {
            var registro = new SqlDataRecord(columnas);
            asignar(registro, elemento);
            yield return registro;
        }
    }
}