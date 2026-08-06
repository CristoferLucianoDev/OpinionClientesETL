using System.Diagnostics;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using OpinionClienteDwh.Data.Dtos;
using OpinionClienteDwh.Data.Interfaces;
using Polly;
using Polly.Retry;

namespace OpinionClienteDwh.Data.Extractors;

public sealed class ApiExtractor : IExtractor<SocialCommentDto>
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiExtractor> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    public ApiExtractor(HttpClient httpClient, ILogger<ApiExtractor> logger)
    {
        _httpClient = httpClient;
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
                        "Reintento {Intento} de ApiExtractor tras fallo transitorio. Esperando {Espera}.",
                        intento, espera);
                });
    }

    public async Task<IReadOnlyList<SocialCommentDto>> ExtraerAsync(CancellationToken cancellationToken)
    {
        var cronometro = Stopwatch.StartNew();

        try
        {
            var resultado = await _retryPolicy.ExecuteAsync(async ct =>
            {
                var respuesta = await _httpClient.GetFromJsonAsync<List<SocialCommentDto>>(
                    "api/SocialComments", ct);

                return (IReadOnlyList<SocialCommentDto>)(respuesta ?? []);
            }, cancellationToken);

            cronometro.Stop();
            _logger.LogInformation(
                "ApiExtractor extrajo {Cantidad} registros en {Tiempo} ms.",
                resultado.Count, cronometro.ElapsedMilliseconds);

            return resultado;
        }
        catch (Exception ex)
        {
            cronometro.Stop();
            _logger.LogError(
                ex,
                "ApiExtractor fallo tras agotar reintentos ({Tiempo} ms transcurridos).",
                cronometro.ElapsedMilliseconds);
            throw;
        }
    }
}
