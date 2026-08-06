using Microsoft.EntityFrameworkCore;
using OpinionClienteDwh.Api.Dao;
using OpinionClienteDwh.Api.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OpinionesOltpContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OpinionesOltp")));

builder.Services.AddScoped<ISocialCommentDao, SocialCommentDao>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();