using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpinionClienteDwh.Data.Interfaces;

namespace OpinionClienteDwh.Data.Staging;

public sealed class StagingReader : IStagingReader
{
    private readonly string _rutaStaging;
    private readonly ILogger<StagingReader> _logger;

    public StagingReader(IConfiguration configuration, ILogger<StagingReader> logger)
    {
        _rutaStaging = configuration["RutasArchivos:Staging"]
            ?? throw new InvalidOperationException("No se encontro 'RutasArchivos:Staging' en la configuracion.");
        _logger = logger;
    }

    public async Task<IReadOnlyList<T>> ReadLastAsync<T>(string prefijo, CancellationToken cancellationToken)
    {
        var archivo = new DirectoryInfo(_rutaStaging)
            .GetFiles($"{prefijo}_*.json")
            .OrderByDescending(f => f.LastWriteTimeUtc)
            .FirstOrDefault();

        if (archivo is null)
        {
            _logger.LogWarning("No se encontro archivo de staging con prefijo '{Prefijo}' en {Ruta}.", prefijo, _rutaStaging);
            return [];
        }

        await using var stream = archivo.OpenRead();
        var datos = await JsonSerializer.DeserializeAsync<List<T>>(stream, cancellationToken: cancellationToken);

        _logger.LogInformation("StagingReader leyo {Cantidad} registros de {Archivo}.", datos?.Count ?? 0, archivo.Name);

        return datos ?? [];
    }
}