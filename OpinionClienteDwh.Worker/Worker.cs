using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpinionClienteDwh.Data.Interfaces;
using OpinionClienteDwh.Data.Interfaces.DaoInterfaces;
using OpinionClienteDwh.Data.Services;

namespace OpinionClienteDwh.Worker;

public sealed class Worker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<Worker> _logger;
    private readonly IHostApplicationLifetime _lifetime;

    public Worker(
        IServiceScopeFactory scopeFactory,
        ILogger<Worker> logger,
        IHostApplicationLifetime lifetime)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _lifetime = lifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Proceso de extraccion iniciado.");

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var orquestador = scope.ServiceProvider.GetRequiredService<OrquestadorExtraccion>();
            var dataLoader = scope.ServiceProvider.GetRequiredService<IDataLoader>();
            var serviceLoadOlap = scope.ServiceProvider.GetRequiredService<IServiceLoadOlap>();

            var resultado = await orquestador.EjecutarAsync(stoppingToken);
            await dataLoader.SaveAsync(resultado, stoppingToken);

            _logger.LogInformation(
                "Extraccion completada. Surveys: {Surveys}, WebReviews: {WebReviews}, SocialComments: {SocialComments}, Clientes: {Clientes}, Productos: {Productos}.",
                resultado.Surveys.Count, resultado.WebReviews.Count, resultado.SocialComments.Count,
                resultado.Clientes.Count, resultado.Productos.Count);

            _logger.LogInformation("Iniciando Transform/Load hacia OpinionesOLAP.");
            await serviceLoadOlap.EjecutarCargaAsync(stoppingToken);
            _logger.LogInformation("Transform/Load completado.");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "El proceso fallo de forma no recuperable.");
        }
        finally
        {
            _lifetime.StopApplication();
        }
    }
}