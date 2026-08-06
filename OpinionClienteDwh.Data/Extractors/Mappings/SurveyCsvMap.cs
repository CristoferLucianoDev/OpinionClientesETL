using CsvHelper.Configuration;
using OpinionClienteDwh.Data.Dtos;

namespace OpinionClienteDwh.Data.Extractors.Mappings;

public sealed class SurveyCsvMap : ClassMap<SurveyDto>
{
    public SurveyCsvMap()
    {
        Map(m => m.IdOriginal).Name("IdOpinion");
        Map(m => m.IdCliente).Name("IdCliente");
        Map(m => m.IdProducto).Name("IdProducto");
        Map(m => m.Fecha).Name("Fecha");
        Map(m => m.Comentario).Name("Comentario");
        Map(m => m.Clasificacion).Name("Clasificación");
        Map(m => m.PuntajeSatisfaccion).Name("PuntajeSatisfacción");
    }
}