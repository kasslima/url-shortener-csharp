using UrlShortener.Application.Abstractions;
using UrlShortener.Application.UseCases;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Tests.Application;

public class CreateShortenedUrlUseCaseTests
{
    [Fact]
    public void Execute_WithValidUrl_ShouldCreateAndStoreShortenedUrl()
    {
        // Arrange
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

        // Act
        var result = useCase.Execute("https://google.com");

        // Assert
        Assert.Equal("abc1234", result.Code);
        Assert.Equal("https://google.com", result.OriginalUrl);
        Assert.Equal(timeProvider.GetUtcNow(), result.CreatedAt);
        Assert.Same(result, repository.GetByCode("abc1234"));
    }
}

internal class FakeRepository : IShortenedUrlRepository
{
    private readonly Dictionary<string, ShortenedUrl> _urls = [];

    public bool Add(ShortenedUrl shortenedUrl)
    {
        return _urls.TryAdd(shortenedUrl.Code, shortenedUrl);
    }

    public ShortenedUrl? GetByCode(string code)
    {
        _urls.TryGetValue(code, out var shortenedUrl);

        return shortenedUrl;
    }

    public bool Delete(string code)
    {
        return _urls.Remove(code);
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