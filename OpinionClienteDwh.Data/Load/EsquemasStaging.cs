using OpinionClienteDwh.Data.Common;

namespace OpinionClienteDwh.Data.Load;

public static class EsquemasStaging
{
    public static readonly EsquemaTablaStaging FactOpinion = new(
        "dbo.Staging_FactOpinion",
        ("IdFecha", typeof(int)),
        ("IdCliente", typeof(int)),
        ("IdProducto", typeof(int)),
        ("IdFuente", typeof(int)),
        ("IdClasificacion", typeof(int)),
        ("Puntaje", typeof(decimal)),
        ("IdOpinionOrigen", typeof(string)));

    public static readonly EsquemaTablaStaging FactPalabraClaveOpinion = new(
        "dbo.Staging_FactPalabraClaveOpinion",
        ("IdOpinion", typeof(int)),
        ("IdPalabraClave", typeof(int)),
        ("IdFecha", typeof(int)),
        ("IdProducto", typeof(int)),
        ("IdClasificacion", typeof(int)),
        ("Frecuencia", typeof(int)));
}