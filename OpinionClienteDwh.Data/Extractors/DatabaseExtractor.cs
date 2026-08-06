using System.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using OpinionClienteDwh.Data.Interfaces;
using OpinionClienteDwh.Data.Interfaces.DaoInterfaces;
using Polly;
using Polly.Retry;

namespace OpinionClienteDwh.Data.Extractors;

public sealed class DatabaseExtractor<T> : IExtractor<T>
{
    private readonly IDao<T> _dao;
    private readonly ILogger<DatabaseExtractor<T>> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    public DatabaseExtractor(IDao<T> dao, ILogger<DatabaseExtractor<T>> logger)
    {
        _dao = dao;
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
                        "Reintento {Intento} de DatabaseExtractor<{Tipo}> tras fallo transitorio. Esperando {Espera}.",
                        intento, typeof(T).Name, espera);
                });
    }

    public async Task<IReadOnlyList<T>> ExtraerAsync(CancellationToken cancellationToken)
    {
        var cronometro = Stopwatch.StartNew();

        try
        {
            var resultado = await _retryPolicy.ExecuteAsync(
                ct => _dao.GetAsync(ct),
                cancellationToken);

            cronometro.Stop();
            _logger.LogInformation(
                "DatabaseExtractor<{Tipo}> extrajo {Cantidad} registros en {Tiempo} ms.",
                typeof(T).Name, resultado.Count, cronometro.ElapsedMilliseconds);

            return resultado;
        }
        catch (Exception ex)
        {
            cronometro.Stop();
            _logger.LogError(
                ex,
                "DatabaseExtractor<{Tipo}> fallo tras agotar reintentos ({Tiempo} ms transcurridos).",
                typeof(T).Name, cronometro.ElapsedMilliseconds);
            throw;
        }
    }
}