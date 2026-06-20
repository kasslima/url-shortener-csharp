using UrlShortener.Application.Abstractions;
using UrlShortener.Application.UseCases;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Tests.Application;

public class CreateShortenedUrlUseCaseTests
{
    [Fact]
    public async Task Execute_WithValidUrl_ShouldCreateAndStoreShortenedUrl()
    {
        var repository = new FakeRepository();
        var codeGenerator = new FakeCodeGenerator("abc1234");
        var timeProvider = new FakeTimeProvider(
            new DateTimeOffset(2026, 6, 19, 12, 0, 0, TimeSpan.Zero)
        );

        var useCase = new CreateShortenedUrlUseCase(
            repository,
            codeGenerator,
            timeProvider
        );

        var result = await useCase.ExecuteAsync("https://google.com");

        Assert.Equal("abc1234", result.Code);
        Assert.Equal("https://google.com", result.OriginalUrl);
        Assert.Equal(timeProvider.GetUtcNow(), result.CreatedAt);

        var stored = await repository.GetByCodeAsync("abc1234");

        Assert.Same(result, stored);
    }
}

internal class FakeRepository : IShortenedUrlRepository
{
    private readonly Dictionary<string, ShortenedUrl> _urls = [];

    public Task<bool> AddAsync(
        ShortenedUrl shortenedUrl,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            _urls.TryAdd(shortenedUrl.Code, shortenedUrl)
        );
    }

    public Task<ShortenedUrl?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        _urls.TryGetValue(code, out var shortenedUrl);

        return Task.FromResult(shortenedUrl);
    }

    public Task<bool> DeleteAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_urls.Remove(code));
    }
}

internal class FakeCodeGenerator : IShortCodeGenerator
{
    private readonly string _code;

    public FakeCodeGenerator(string code)
    {
        _code = code;
    }

    public string Generate()
    {
        return _code;
    }
}

internal class FakeTimeProvider : TimeProvider
{
    private readonly DateTimeOffset _dateTime;

    public FakeTimeProvider(DateTimeOffset dateTime)
    {
        _dateTime = dateTime;
    }

    public override DateTimeOffset GetUtcNow()
    {
        return _dateTime;
    }
}