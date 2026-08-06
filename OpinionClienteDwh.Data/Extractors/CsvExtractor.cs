using System.Diagnostics;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using OpinionClienteDwh.Data.Interfaces;

namespace OpinionClienteDwh.Data.Extractors;

public sealed class CsvExtractor<T> : IExtractor<T>
{
    private readonly string _rutaArchivo;
    private readonly ClassMap<T> _mapa;
    private readonly ILogger<CsvExtractor<T>> _logger;

    public CsvExtractor(string rutaArchivo, ClassMap<T> mapa, ILogger<CsvExtractor<T>> logger)
    {
        _rutaArchivo = rutaArchivo;
        _mapa = mapa;
        _logger = logger;
    }

    public async Task<IReadOnlyList<T>> ExtraerAsync(CancellationToken cancellationToken)
    {
        var cronometro = Stopwatch.StartNew();
        var resultado = new List<T>();

        try
        {
            using var lector = new StreamReader(_rutaArchivo);
            using var csv = new CsvReader(lector, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap(_mapa);

            await foreach (var registro in csv.GetRecordsAsync<T>(cancellationToken))
            {
                resultado.Add(registro);
            }

            cronometro.Stop();
            _logger.LogInformation(
                "CsvExtractor<{Tipo}> extrajo {Cantidad} registros de {Archivo} en {Tiempo} ms.",
                typeof(T).Name, resultado.Count, _rutaArchivo, cronometro.ElapsedMilliseconds);

            return resultado;
        }
        catch (Exception ex)
        {
            cronometro.Stop();
            _logger.LogError(
                ex,
                "CsvExtractor<{Tipo}> fallo leyendo {Archivo} ({Tiempo} ms transcurridos).",
                typeof(T).Name, _rutaArchivo, cronometro.ElapsedMilliseconds);
            throw;
        }
    }
}