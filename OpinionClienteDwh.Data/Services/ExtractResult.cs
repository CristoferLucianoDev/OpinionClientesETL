using OpinionClienteDwh.Data.Dtos;

namespace OpinionClienteDwh.Data.Services;

public sealed record ExtractResult
{
    public required IReadOnlyList<SurveyDto> Surveys { get; init; }
    public required IReadOnlyList<WebReviewDto> WebReviews { get; init; }
    public required IReadOnlyList<SocialCommentDto> SocialComments { get; init; }
    public required IReadOnlyList<ClienteDto> Clientes { get; init; }
    public required IReadOnlyList<ProductoDto> Productos { get; init; }
}