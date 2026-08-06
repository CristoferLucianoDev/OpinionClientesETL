using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpinionClienteDwh.Data.Dao;
using OpinionClienteDwh.Data.Dtos;
using OpinionClienteDwh.Data.Extractors;
using OpinionClienteDwh.Data.Extractors.Mappings;
using OpinionClienteDwh.Data.Interfaces;
using OpinionClienteDwh.Data.Interfaces.DaoInterfaces;
using OpinionClienteDwh.Data.Services;
using OpinionClienteDwh.Data.Validators;
using OpinionClienteDwh.Worker;
using System.Data;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddScoped<IDao<WebReviewDto>, WebReviewDao>();

builder.Services.AddScoped<IExtractor<SurveyDto>>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var logger = sp.GetRequiredService<ILogger<CsvExtractor<SurveyDto>>>();
    var ruta = configuration["RutasArchivos:Surveys"]
        ?? throw new InvalidOperationException("No se encontro 'RutasArchivos:Surveys' en la configuracion.");

    return new CsvExtractor<SurveyDto>(ruta, new SurveyCsvMap(), logger);
});

builder.Services.AddScoped<IExtractor<WebReviewDto>, DatabaseExtractor<WebReviewDto>>();

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

builder.Services.AddScoped<IDataLoader, DataLoader>();
builder.Services.AddScoped<OrquestadorExtraccion>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();