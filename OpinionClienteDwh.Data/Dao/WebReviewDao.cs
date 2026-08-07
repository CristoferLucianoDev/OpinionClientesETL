using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpinionClienteDwh.Data.Common;
using OpinionClienteDwh.Data.Dtos;
using OpinionClienteDwh.Data.Interfaces.DaoInterfaces;

namespace OpinionClienteDwh.Data.Dao;

public sealed class WebReviewDao(IConfiguration configuration, ILogger<WebReviewDao> logger)
    : SqlServerConnection(configuration, "OpinionesOltp"), IDao<WebReviewDto>
{
    private readonly ILogger<WebReviewDao> _logger = logger;

    public async Task<IReadOnlyList<WebReviewDto>> GetAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await ExecuteReaderAsync(
                "dbo.SP_ObtenerWebReviews",
                command => { },
                reader => new WebReviewDto
                {
                    IdOriginal = reader.GetString(reader.GetOrdinal("IdOriginal")),
                    IdCliente = reader.GetInt32(reader.GetOrdinal("IdCliente")),
                    IdProducto = reader.GetInt32(reader.GetOrdinal("IdProducto")),
                    Fecha = reader.GetDateTime(reader.GetOrdinal("Fecha")),
                    Comentario = reader.GetString(reader.GetOrdinal("Comentario")),
                    Rating = reader.GetDecimal(reader.GetOrdinal("Rating"))
                },
                cancellationToken);
        }
        catch (SqlException ex)
        {
            _logger.LogError(
                ex,
                "Error de SQL Server al ejecutar SP_ObtenerWebReviews. Numero de error: {NumeroError}.",
                ex.Number);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado en WebReviewDao.ObtenerWebReviewsAsync.");
            throw;
        }
    }
}
