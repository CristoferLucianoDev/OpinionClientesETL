using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpinionClienteDwh.Api.Models;
using OpinionClienteDwh.Api.Persistence;

namespace OpinionClienteDwh.Api.Dao;

public sealed class SocialCommentDao : ISocialCommentDao
{
    private readonly OpinionesOltpContext _context;
    private readonly ILogger<SocialCommentDao> _logger;

    public SocialCommentDao(OpinionesOltpContext context, ILogger<SocialCommentDao> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IReadOnlyList<SocialCommentResponse>> GetSocialCommentsAsync(CancellationToken cancellationToken)
    {
        var socialComments = new List<SocialCommentResponse>();

        try
        {
            var query = await(from s in _context.SocialComments
                                   select new SocialCommentResponse
                                   {
                                       IdOriginal = s.IdOriginal,
                                       IdCliente = s.IdCliente,
                                       IdProducto = s.IdProducto,
                                       Fecha = s.Fecha,
                                       Comentario = s.Comentario
                                   }).ToListAsync(cancellationToken);
            return query;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado en SocialCommentDao.ObtenerSocialCommentsAsync.");
            throw;
        }
    }
}