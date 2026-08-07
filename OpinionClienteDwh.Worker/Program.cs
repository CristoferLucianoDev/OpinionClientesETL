using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpinionClienteDwh.Data.Dao;
using OpinionClienteDwh.Data.Dtos;
using OpinionClienteDwh.Data.Extractors;
using OpinionClienteDwh.Data.Extractors.Mappings;
using OpinionClienteDwh.Data.Interfaces;
using OpinionClienteDwh.Data.Interfaces.DaoInterfaces;
using OpinionClienteDwh.Data.Load.Daos;
using OpinionClienteDwh.Data.Load.Services;
using OpinionClienteDwh.Data.Nlp;
using OpinionClienteDwh.Data.Cache;
using OpinionClienteDwh.Data.Persistence;
using OpinionClienteDwh.Data.Services;
using OpinionClienteDwh.Data.Staging;
using OpinionClienteDwh.Data.Validators;
using OpinionClienteDwh.Worker;

var builder = Host.CreateApplicationBuilder(args);

// ---- Extract: fuentes ----

builder.Services.AddDbContextFactory<OpinionesOltpReadContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OpinionesOltp")));

builder.Services.AddScoped<IDao<WebReviewDto>, WebReviewDao>();
builder.Services.AddScoped<IDao<ClienteDto>, ClienteDao>();
builder.Services.AddScoped<IDao<ProductoDto>, ProductoDao>();

builder.Services.AddScoped<IExtractor<SurveyDto>>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var logger = sp.GetRequiredService<ILogger<CsvExtractor<SurveyDto>>>();
    var ruta = configuration["RutasArchivos:Surveys"]
        ?? throw new InvalidOperationException("No se encontro 'RutasArchivos:Surveys' en la configuracion.");

    return new CsvExtractor<SurveyDto>(ruta, new SurveyCsvMap(), logger);
});

builder.Services.AddScoped<IExtractor<WebReviewDto>, DatabaseExtractor<WebReviewDto>>();
builder.Services.AddScoped<IExtractor<ClienteDto>, DatabaseExtractor<ClienteDto>>();
builder.Services.AddScoped<IExtractor<ProductoDto>, DatabaseExtractor<ProductoDto>>();

builder.Services.AddHttpClient("ApiMock", client =>
{
    var baseUrl = builder.Configuration["ApiMock:BaseUrl"]
        ?? throw new InvalidOperationException("No se encontro 'ApiMock:BaseUrl' en la configuracion.");
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddScoped<IExtractor<SocialCommentDto>>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var client = factory.CreateClient("ApiMock");
    var logger = sp.GetRequiredService<ILogger<ApiExtractor<SocialCommentDto>>>();

    return new ApiExtractor<SocialCommentDto>(client, "api/SocialComments", logger);
});

builder.Services.AddScoped<IValidator<SurveyDto>, SurveyValidator>();
builder.Services.AddScoped<IValidator<WebReviewDto>, WebReviewValidator>();
builder.Services.AddScoped<IValidator<SocialCommentDto>, SocialCommentValidator>();
builder.Services.AddScoped<IValidator<ClienteDto>, ClienteValidator>();
builder.Services.AddScoped<IValidator<ProductoDto>, ProductoValidator>();

builder.Services.AddScoped<IDataLoader, DataLoader>();
builder.Services.AddScoped<OrquestadorExtraccion>();

// ---- Transform / Load: hacia OpinionesOLAP ----

builder.Services.AddScoped<IDaoDimensionKey, DaoDimensionKey>();
builder.Services.AddScoped<IDaoFactOpinion, DaoFactOpinion>();
builder.Services.AddScoped<IDaoFactPalabraClaveOpinion, DaoFactPalabraClaveOpinion>();
builder.Services.AddScoped<IDaoMergeDimensiones, DaoMergeDimensiones>();

builder.Services.AddScoped<IDimensionKeyCache, DimensionKeyCache>();
builder.Services.AddSingleton<ITokenizadorComentarios, TokenizadorComentarios>();

builder.Services.AddScoped<IStagingReader, StagingReader>();
builder.Services.AddScoped<ILectorOpinionesConsolidadas, LectorOpinionesConsolidadas>();

builder.Services.AddScoped<IServiceMergeDimensiones, ServiceMergeDimensiones>();
builder.Services.AddScoped<IServiceCargaFactOpinion, ServiceCargaFactOpinion>();
builder.Services.AddScoped<IServiceCargaFactPalabraClaveOpinion, ServiceCargaFactPalabraClaveOpinion>();
builder.Services.AddScoped<IServiceLoadOlap, ServiceLoadOlap>();

// ---- Host ----

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();