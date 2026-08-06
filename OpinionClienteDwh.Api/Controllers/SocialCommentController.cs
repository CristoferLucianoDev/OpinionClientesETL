using Microsoft.AspNetCore.Mvc;
using OpinionClienteDwh.Api.Dao;

namespace OpinionClienteDwh.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SocialCommentsController : ControllerBase
{
    private readonly ISocialCommentDao _socialCommentDao;

    public SocialCommentsController(ISocialCommentDao socialCommentDao)
    {
        _socialCommentDao = socialCommentDao;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var resultado = await _socialCommentDao.GetSocialCommentsAsync(cancellationToken);
        return Ok(resultado);
    }
}