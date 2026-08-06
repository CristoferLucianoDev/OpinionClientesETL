using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpinionClienteDwh.Data.Dtos;
using OpinionClienteDwh.Data.Interfaces;
using System.Diagnostics;
using System.Formats.Asn1;
using System.Globalization;

namespace OpinionClienteDwh.Data.Extractors;

public sealed class CsvExtractor : IExtractor<SurveyDto>
{
    private readonly string _rutaArchivo;
    private readonly ILogger<CsvExtractor> _logger;

    public CsvExtractor(IConfiguration configuration, ILogger<CsvExtractor> logger)
    {
        _rutaArchivo = configuration["RutasArchivos:Surveys"]
            ?? throw new InvalidOperationException(
                "No se encontro la ruta 'RutasArchivos:Surveys' en la configuracion.");
        _logger = logger;
    }

    public async Task<IReadOnlyList<SurveyDto>> ExtraerAsync(CancellationToken cancellationToken)
    {
        var cronometro = Stopwatch.StartNew();
        var resultado = new List<SurveyDto>();

        try
        {
            using var lector = new StreamReader(_rutaArchivo);
            using var csv = new CsvReader(lector, CultureInfo.InvariantCulture);

            await foreach (var registro in csv.GetRecordsAsync<SurveyCsvRecord>(cancellationToken))
            {
                resultado.Add(new SurveyDto
                {
                    IdOriginal = registro.IdOpinion.ToString(CultureInfo.InvariantCulture),
                    IdCliente = registro.IdCliente,
                    IdProducto = registro.IdProducto,
                    Fecha = registro.Fecha,
                    Comentario = registro.Comentario,
                    PuntajeSatisfaccion = registro.PuntajeSatisfaccion,
                    Clasificacion = registro.Clasificacion
                });
            }

            cronometro.Stop();
            _logger.LogInformation(
                "CsvExtractor extrajo {Cantidad} registros de {Archivo} en {Tiempo} ms.",
                resultado.Count, _rutaArchivo, cronometro.ElapsedMilliseconds);

            return resultado;
        }
        catch (Exception ex)
        {
            cronometro.Stop();
            _logger.LogError(
                ex,
                "CsvExtractor fallo leyendo {Archivo} ({Tiempo} ms transcurridos).",
                _rutaArchivo, cronometro.ElapsedMilliseconds);
            throw;
        }
    }

    private sealed class SurveyCsvRecord
    {
        [Name("IdOpinion")]
        public required int IdOpinion { get; init; }

        [Name("IdCliente")]
        public required int IdCliente { get; init; }

        [Name("IdProducto")]
        public required int IdProducto { get; init; }

        [Name("Clasificacion")]
        public required string Clasificacion { get; init; }

        [Name("Comentario")]
        public required string Comentario { get; init; }

        [Name("Fecha")]
        public required DateTime Fecha { get; init; }

        [Name("PuntajeSatisfaccion")]
        public required decimal PuntajeSatisfaccion { get; init; }
    }
}