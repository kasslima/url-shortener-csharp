using UrlShortener.Application.Abstractions;
using UrlShortener.Application.UseCases;
using UrlShortener.Infrastructure.Repositories;
using UrlShortener.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSingleton<IShortenedUrlRepository, InMemoryShortenedUrlRepository>();

builder.Services.AddSingleton<IShortCodeGenerator, RandomShortCodeGenerator>();

builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddScoped<CreateShortenedUrlUseCase>();
builder.Services.AddScoped<GetShortenedUrlUseCase>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();