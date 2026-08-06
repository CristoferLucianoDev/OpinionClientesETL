using System.Diagnostics;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using OpinionClienteDwh.Data.Interfaces;
using Polly;
using Polly.Retry;

namespace OpinionClienteDwh.Data.Extractors;

public sealed class ApiExtractor<T> : IExtractor<T>
{
    private readonly HttpClient _httpClient;
    private readonly string _endpoint;
    private readonly ILogger<ApiExtractor<T>> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    public ApiExtractor(HttpClient httpClient, string endpoint, ILogger<ApiExtractor<T>> logger)
    {
        _httpClient = httpClient;
        _endpoint = endpoint;
        _logger = logger;

        _retryPolicy = Policy
            .Handle<HttpRequestException>()
            .Or<TaskCanceledException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: intento => TimeSpan.FromSeconds(Math.Pow(2, intento)),
                onRetry: (exception, espera, intento, _) =>
                {
                    _logger.LogWarning(
                        exception,
                        "Reintento {Intento} de ApiExtractor<{Tipo}> tras fallo transitorio. Esperando {Espera}.",
                        intento, typeof(T).Name, espera);
                });
    }

    public async Task<IReadOnlyList<T>> ExtraerAsync(CancellationToken cancellationToken)
    {
        var cronometro = Stopwatch.StartNew();

        try
        {
            var resultado = await _retryPolicy.ExecuteAsync(async ct =>
            {
                var respuesta = await _httpClient.GetFromJsonAsync<List<T>>(_endpoint, ct);
                return (IReadOnlyList<T>)(respuesta ?? []);
            }, cancellationToken);

            cronometro.Stop();
            _logger.LogInformation(
                "ApiExtractor<{Tipo}> extrajo {Cantidad} registros de {Endpoint} en {Tiempo} ms.",
                typeof(T).Name, resultado.Count, _endpoint, cronometro.ElapsedMilliseconds);

            return resultado;
        }
        catch (Exception ex)
        {
            cronometro.Stop();
            _logger.LogError(
                ex,
                "ApiExtractor<{Tipo}> fallo tras agotar reintentos ({Tiempo} ms transcurridos).",
                typeof(T).Name, cronometro.ElapsedMilliseconds);
            throw;
        }
    }
}