using OpinionClienteDwh.Data.Dtos;
using OpinionClienteDwh.Data.Interfaces;
using OpinionClienteDwh.Data.Models;

namespace OpinionClienteDwh.Data.Services;

public sealed class LectorOpinionesConsolidadas(IStagingReader stagingReader) : ILectorOpinionesConsolidadas
{
    private readonly IStagingReader _stagingReader = stagingReader;

    public async Task<IReadOnlyList<OpinionConsolidada>> ObtenerAsync(CancellationToken cancellationToken)
    {
        var surveys = await _stagingReader.ReadLastAsync<SurveyDto>("surveys", cancellationToken);
        var webReviews = await _stagingReader.ReadLastAsync<WebReviewDto>("webreviews", cancellationToken);
        var socialComments = await _stagingReader.ReadLastAsync<SocialCommentDto>("socialcomments", cancellationToken);

        var resultado = new List<OpinionConsolidada>(surveys.Count + webReviews.Count + socialComments.Count);

        resultado.AddRange(webReviews.Select(w => new OpinionConsolidada
        {
            Origen = "WebReview",
            IdOpinionOrigen = w.IdOriginal,
            IdCliente = w.IdCliente,
            IdProducto = w.IdProducto,
            Fecha = DateOnly.FromDateTime(w.Fecha),
            Comentario = w.Comentario,
            Clasificacion = null,
            Puntaje = w.Rating
        }));

        resultado.AddRange(socialComments.Select(s => new OpinionConsolidada
        {
            Origen = "SocialComment",
            IdOpinionOrigen = s.IdOriginal,
            IdCliente = s.IdCliente,
            IdProducto = s.IdProducto,
            Fecha = DateOnly.FromDateTime(s.Fecha),
            Comentario = s.Comentario,
            Clasificacion = null,
            Puntaje = null
        }));

        resultado.AddRange(surveys.Select(s => new OpinionConsolidada
        {
            Origen = "Survey",
            IdOpinionOrigen = s.IdOriginal,
            IdCliente = s.IdCliente,
            IdProducto = s.IdProducto,
            Fecha = DateOnly.FromDateTime(s.Fecha),
            Comentario = s.Comentario,
            Clasificacion = s.Clasificacion,
            Puntaje = s.PuntajeSatisfaccion
        }));

        return resultado;
    }
}