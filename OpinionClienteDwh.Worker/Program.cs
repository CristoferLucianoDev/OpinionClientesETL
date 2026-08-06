using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpinionClienteDwh.Data.Dao;
using OpinionClienteDwh.Data.Dtos;
using OpinionClienteDwh.Data.Extractors;
using OpinionClienteDwh.Data.Interfaces;
using OpinionClienteDwh.Data.Interfaces.DaoInterfaces;
using OpinionClienteDwh.Data.Services;
using OpinionClienteDwh.Data.Validators;
using OpinionClienteDwh.Worker;
using System.Data;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddScoped<IWebReviewDao, WebReviewDao>();

builder.Services.AddScoped<IExtractor<SurveyDto>, CsvExtractor>();
builder.Services.AddScoped<IExtractor<WebReviewDto>, DatabaseExtractor>();
builder.Services.AddHttpClient<IExtractor<SocialCommentDto>, ApiExtractor>(client =>
{
    var baseUrl = builder.Configuration["ApiMock:BaseUrl"]
        ?? throw new InvalidOperationException("No se encontro 'ApiMock:BaseUrl' en la configuracion.");
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddScoped<IValidator<SurveyDto>, SurveyValidator>();
builder.Services.AddScoped<IValidator<WebReviewDto>, WebReviewValidator>();
builder.Services.AddScoped<IValidator<SocialCommentDto>, SocialCommentValidator>();

builder.Services.AddScoped<IDataLoader, DataLoader>();
builder.Services.AddScoped<OrquestadorExtraccion>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();