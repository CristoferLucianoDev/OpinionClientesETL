using OpinionClienteDwh.Api.Models;

namespace OpinionClienteDwh.Api.Dao;

public interface ISocialCommentDao
{
    Task<IReadOnlyList<SocialCommentResponse>> GetSocialCommentsAsync(CancellationToken cancellationToken);
}