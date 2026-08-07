using System.Data;

namespace OpinionClienteDwh.Data.Common;

public sealed class EsquemaTablaStaging
{
    private readonly (string Nombre, Type Tipo)[] _columnas;

    public EsquemaTablaStaging(string nombreTabla, params (string Nombre, Type Tipo)[] columnas)
    {
        NombreTabla = nombreTabla;
        _columnas = columnas;
    }

    public string NombreTabla { get; }

    public IEnumerable<(string Origen, string Destino)> Mapeos
        => _columnas.Select(columna => (columna.Nombre, columna.Nombre));

    public DataTable CrearTabla()
    {
        var tabla = new DataTable();

        foreach (var (nombre, tipo) in _columnas)
            tabla.Columns.Add(nombre, tipo);

        return tabla;
    }
}