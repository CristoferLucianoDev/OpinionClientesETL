using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpinionClienteDwh.Data.Interfaces;

namespace OpinionClienteDwh.Data.Services;

public sealed class DataLoader : IDataLoader
{
    private readonly string _rutaStaging;
    private readonly ILogger<DataLoader> _logger;

    private static readonly JsonSerializerOptions OpcionesJson = new()
    {
        WriteIndented = true
    };

    public DataLoader(IConfiguration configuration, ILogger<DataLoader> logger)
    {
        _rutaStaging = configuration["RutasArchivos:Staging"]
            ?? throw new InvalidOperationException(
                "No se encontro la ruta 'RutasArchivos:Staging' en la configuracion.");
        _logger = logger;
    }

    public async Task SaveAsync(ExtractResult resultado, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(_rutaStaging);

        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

        await EscribirArchivoAsync("surveys", timestamp, resultado.Surveys, cancellationToken);
        await EscribirArchivoAsync("webreviews", timestamp, resultado.WebReviews, cancellationToken);
        await EscribirArchivoAsync("socialcomments", timestamp, resultado.SocialComments, cancellationToken);
    }

    private async Task EscribirArchivoAsync<T>(
        string nombreFuente,
        string timestamp,
        IReadOnlyList<T> datos,
        CancellationToken cancellationToken)
    {
        var nombreArchivo = $"{nombreFuente}_{timestamp}.json";
        var rutaCompleta = Path.Combine(_rutaStaging, nombreArchivo);

        try
        {
            await using var stream = File.Create(rutaCompleta);
            await JsonSerializer.SerializeAsync(stream, datos, OpcionesJson, cancellationToken);

            _logger.LogInformation(
                "DataLoader escribio {Cantidad} registros de {Fuente} en {Archivo}.",
                datos.Count, nombreFuente, nombreArchivo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DataLoader fallo escribiendo {Archivo}.", nombreArchivo);
            throw;
        }
    }
}