using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Application.Abstractions;
using UrlShortener.Application.UseCases;
using UrlShortener.Infrastructure.Persistence;
using UrlShortener.Infrastructure.Repositories;
using UrlShortener.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var hasJsonError = context.ModelState.Any(entry =>
                entry.Key.StartsWith("$") && entry.Value?.Errors.Count > 0);

            var errors = context.ModelState
                .Where(entry => entry.Value?.Errors.Count > 0)
                .Where(entry => !hasJsonError || entry.Key != "request")
                .SelectMany(entry => entry.Value!.Errors.Select(error => new
                {
                    Field = entry.Key switch
                    {
                        "$" => "json",
                        _ when entry.Key.StartsWith("$.") => entry.Key[2..],
                        _ => entry.Key
                    },
                    Message = entry.Key switch
                    {
                        "$" => "The request body contains invalid JSON.",
                        _ when entry.Key.StartsWith("$.") || error.Exception is not null
                            => "The provided value has an invalid format.",
                        _ => error.ErrorMessage
                    }
                }))
                .GroupBy(error => error.Field)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.Message).Distinct().ToArray()
                );

            return new BadRequestObjectResult(new
            {
                message = "The request could not be processed.",
                errors
            });
        };
    });
builder.Services.AddOpenApi();

builder.Services.AddDbContext<UrlShortenerDbContext>(options =>
{
    var connectionString = builder.Configuration
        .GetConnectionString("DefaultConnection");

    options.UseNpgsql(connectionString);
});

builder.Services.AddScoped<IShortenedUrlRepository,
    PostgresShortenedUrlRepository>();

builder.Services.AddSingleton<IShortCodeGenerator,
    RandomShortCodeGenerator>();

builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddScoped<CreateShortenedUrlUseCase>();
builder.Services.AddScoped<GetAllShortenedUrlUseCase>();
builder.Services.AddScoped<GetShortenedUrlUseCase>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
