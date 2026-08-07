using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpinionClienteDwh.Data.Dtos;
using OpinionClienteDwh.Data.Interfaces.DaoInterfaces;
using OpinionClienteDwh.Data.Persistence;

namespace OpinionClienteDwh.Data.Dao;

public sealed class ClienteDao : IDao<ClienteDto>
{
    private readonly IDbContextFactory<OpinionesOltpReadContext> _contextFactory;
    private readonly ILogger<ClienteDao> _logger;

    public ClienteDao(IDbContextFactory<OpinionesOltpReadContext> contextFactory, ILogger<ClienteDao> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task<IReadOnlyList<ClienteDto>> GetAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

            return await context.Clientes
                .AsNoTracking()
                .Select(c => new ClienteDto
                {
                    IdCliente = c.IdCliente,
                    Nombre = c.Nombre,
                    Email = c.Email
                })
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado en ClienteDao.ObtenerAsync.");
            throw;
        }
    }
}