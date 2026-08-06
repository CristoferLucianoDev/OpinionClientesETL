using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpinionClienteDwh.Data.Dtos;
using OpinionClienteDwh.Data.Interfaces.DaoInterfaces;
using System.Data;

namespace OpinionClienteDwh.Data.Dao;

public sealed class WebReviewDao : IWebReviewDao
{
    private readonly string _connectionString;
    private readonly ILogger<WebReviewDao> _logger;

    public WebReviewDao(IConfiguration configuration, ILogger<WebReviewDao> logger)
    {
        _connectionString = configuration.GetConnectionString("OpinionesOltp")
            ?? throw new InvalidOperationException(
                "No se encontro la cadena de conexion 'OpinionesOltp' en la configuracion.");
        _logger = logger;
    }

    public async Task<IReadOnlyList<WebReviewDto>> GetWebReviewsAsync(CancellationToken cancellationToken)
    {
        var resultado = new List<WebReviewDto>();

        try
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand("dbo.SP_ObtenerWebReviews", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                resultado.Add(new WebReviewDto
                {
                    IdOriginal = reader.GetString(reader.GetOrdinal("IdOriginal")),
                    IdCliente = reader.GetInt32(reader.GetOrdinal("IdCliente")),
                    IdProducto = reader.GetInt32(reader.GetOrdinal("IdProducto")),
                    Fecha = reader.GetDateTime(reader.GetOrdinal("Fecha")),
                    Comentario = reader.GetString(reader.GetOrdinal("Comentario")),
                    Rating = reader.GetDecimal(reader.GetOrdinal("Rating"))
                });
            }

            return resultado;
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