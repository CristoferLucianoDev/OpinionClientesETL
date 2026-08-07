using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpinionClienteDwh.Data.Dtos;
using OpinionClienteDwh.Data.Interfaces.DaoInterfaces;
using OpinionClienteDwh.Data.Persistence;

namespace OpinionClienteDwh.Data.Dao;

public sealed class ProductoDao : IDao<ProductoDto>
{
    private readonly IDbContextFactory<OpinionesOltpReadContext> _contextFactory;
    private readonly ILogger<ProductoDao> _logger;

    public ProductoDao(IDbContextFactory<OpinionesOltpReadContext> contextFactory, ILogger<ProductoDao> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task<IReadOnlyList<ProductoDto>> GetAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

            return await (from p in context.Productos.AsNoTracking()
                          join pc in context.ProductoCategorias.AsNoTracking()
                              on p.IdProducto equals pc.IdProducto
                          join c in context.Categorias.AsNoTracking()
                              on pc.IdCategoria equals c.IdCategoria
                          select new ProductoDto
                          {
                              IdProducto = p.IdProducto,
                              Nombre = p.Nombre,
                              Categoria = c.Nombre
                          }).ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado en ProductoDao.ObtenerAsync.");
            throw;
        }
    }
}