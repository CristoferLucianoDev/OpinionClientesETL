using OpinionClienteDwh.Data.Dtos;

namespace OpinionClienteDwh.Data.Interfaces.DaoInterfaces;

public interface IWebReviewDao
{
    Task<IReadOnlyList<WebReviewDto>> GetWebReviewsAsync(CancellationToken cancellationToken);
}