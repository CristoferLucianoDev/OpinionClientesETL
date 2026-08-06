using System.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using OpinionClienteDwh.Data.Dtos;
using OpinionClienteDwh.Data.Interfaces;
using OpinionClienteDwh.Data.Interfaces.DaoInterfaces;
using Polly;
using Polly.Retry;

namespace OpinionClienteDwh.Data.Extractors;

public sealed class DatabaseExtractor : IExtractor<WebReviewDto>
{
    private readonly IWebReviewDao _webReviewDao;
    private readonly ILogger<DatabaseExtractor> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    public DatabaseExtractor(IWebReviewDao webReviewDao, ILogger<DatabaseExtractor> logger)
    {
        _webReviewDao = webReviewDao;
        _logger = logger;

        _retryPolicy = Policy
            .Handle<SqlException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: intento => TimeSpan.FromSeconds(Math.Pow(2, intento)),
                onRetry: (exception, espera, intento, _) =>
                {
                    _logger.LogWarning(
                        exception,
                        "Reintento {Intento} de DatabaseExtractor tras fallo transitorio. Esperando {Espera}.",
                        intento, espera);
                });
    }

    public async Task<IReadOnlyList<WebReviewDto>> ExtraerAsync(CancellationToken cancellationToken)
    {
        var cronometro = Stopwatch.StartNew();

        try
        {
            var resultado = await _retryPolicy.ExecuteAsync(
                ct => _webReviewDao.GetWebReviewsAsync(ct),
                cancellationToken);

            cronometro.Stop();
            _logger.LogInformation(
                "DatabaseExtractor extrajo {Cantidad} registros en {Tiempo} ms.",
                resultado.Count, cronometro.ElapsedMilliseconds);

            return resultado;
        }
        catch (Exception ex)
        {
            cronometro.Stop();
            _logger.LogError(
                ex,
                "DatabaseExtractor fallo tras agotar reintentos ({Tiempo} ms transcurridos).",
                cronometro.ElapsedMilliseconds);
            throw;
        }
    }
}