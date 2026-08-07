using Microsoft.Extensions.Logging;
using OpinionClienteDwh.Data.Dtos;
using OpinionClienteDwh.Data.Interfaces;

namespace OpinionClienteDwh.Data.Services;

public sealed class OrquestadorExtraccion
{
    private readonly IExtractor<SurveyDto> _csvExtractor;
    private readonly IExtractor<WebReviewDto> _databaseExtractor;
    private readonly IExtractor<SocialCommentDto> _apiExtractor;
    private readonly IExtractor<ClienteDto> _clienteExtractor;
    private readonly IExtractor<ProductoDto> _productoExtractor;
    private readonly IValidator<SurveyDto> _surveyValidator;
    private readonly IValidator<WebReviewDto> _webReviewValidator;
    private readonly IValidator<SocialCommentDto> _socialCommentValidator;
    private readonly IValidator<ClienteDto> _clienteValidator;
    private readonly IValidator<ProductoDto> _productoValidator;
    private readonly ILogger<OrquestadorExtraccion> _logger;

    public OrquestadorExtraccion(
        IExtractor<SurveyDto> csvExtractor,
        IExtractor<WebReviewDto> databaseExtractor,
        IExtractor<SocialCommentDto> apiExtractor,
        IExtractor<ClienteDto> clienteExtractor,
        IExtractor<ProductoDto> productoExtractor,
        IValidator<SurveyDto> surveyValidator,
        IValidator<WebReviewDto> webReviewValidator,
        IValidator<SocialCommentDto> socialCommentValidator,
        IValidator<ClienteDto> clienteValidator,
        IValidator<ProductoDto> productoValidator,
        ILogger<OrquestadorExtraccion> logger)
    {
        _csvExtractor = csvExtractor;
        _databaseExtractor = databaseExtractor;
        _apiExtractor = apiExtractor;
        _clienteExtractor = clienteExtractor;
        _productoExtractor = productoExtractor;
        _surveyValidator = surveyValidator;
        _webReviewValidator = webReviewValidator;
        _socialCommentValidator = socialCommentValidator;
        _clienteValidator = clienteValidator;
        _productoValidator = productoValidator;
        _logger = logger;
    }

    public async Task<ExtractResult> EjecutarAsync(CancellationToken cancellationToken)
    {
        var tareaSurveys = ExtraerYValidarAsync(_csvExtractor, _surveyValidator, "Surveys (CSV)", cancellationToken);
        var tareaWebReviews = ExtraerYValidarAsync(_databaseExtractor, _webReviewValidator, "WebReviews (BD)", cancellationToken);
        var tareaSocialComments = ExtraerYValidarAsync(_apiExtractor, _socialCommentValidator, "SocialComments (API)", cancellationToken);
        var tareaClientes = ExtraerYValidarAsync(_clienteExtractor, _clienteValidator, "Clientes (BD)", cancellationToken);
        var tareaProductos = ExtraerYValidarAsync(_productoExtractor, _productoValidator, "Productos (BD)", cancellationToken);

        await Task.WhenAll(tareaSurveys, tareaWebReviews, tareaSocialComments, tareaClientes, tareaProductos);

        return new ExtractResult
        {
            Surveys = tareaSurveys.Result,
            WebReviews = tareaWebReviews.Result,
            SocialComments = tareaSocialComments.Result,
            Clientes = tareaClientes.Result,
            Productos = tareaProductos.Result
        };
    }

    private async Task<IReadOnlyList<T>> ExtraerYValidarAsync<T>(
        IExtractor<T> extractor,
        IValidator<T> validator,
        string nombreFuente,
        CancellationToken cancellationToken)
    {
        try
        {
            var extraidos = await extractor.ExtraerAsync(cancellationToken);

            var validos = new List<T>();
            var totalRechazados = 0;

            foreach (var item in extraidos)
            {
                if (validator.EsValido(item, out var motivoRechazo))
                {
                    validos.Add(item);
                }
                else
                {
                    totalRechazados++;
                    _logger.LogWarning(
                        "{Fuente}: registro rechazado en validacion. Motivo: {Motivo}.",
                        nombreFuente, motivoRechazo);
                }
            }

            _logger.LogInformation(
                "{Fuente}: {TotalValidos} validos, {TotalRechazados} rechazados de {TotalExtraidos} extraidos.",
                nombreFuente, validos.Count, totalRechazados, extraidos.Count);

            return validos;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "{Fuente}: extraccion fallo tras agotar reintentos. Se continua con las demas fuentes sin datos de esta.",
                nombreFuente);

            return [];
        }
    }
}