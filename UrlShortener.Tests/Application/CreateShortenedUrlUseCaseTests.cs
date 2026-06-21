using UrlShortener.Application.Abstractions;
using UrlShortener.Application.UseCases;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Tests.Application;

public class CreateShortenedUrlUseCaseTests
{
    [Fact]
    public async Task Execute_WithValidUrl_ShouldReturnShortenedUrl()
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
        Assert.Equal(0, result.AccessCount);
    }

    [Fact]
    public async Task Execute_WithValidUrl_ShouldStoreShortenedUrl()
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

        var stored = await repository.GetByCodeAsync("abc1234");

        Assert.NotNull(stored);
        Assert.Same(result, stored);
    }

    [Fact]
    public async Task GetAll_ShouldReturnStoredShortenedUrls()
    {
        var repository = new FakeRepository();
        var codeGenerator = new FakeCodeGenerator("abc1234");
        var timeProvider = new FakeTimeProvider(
            new DateTimeOffset(2026, 6, 19, 12, 0, 0, TimeSpan.Zero)
        );

        var createUseCase = new CreateShortenedUrlUseCase(
            repository,
            codeGenerator,
            timeProvider
        );

        var getAllUseCase = new GetAllShortenedUrlUseCase(repository);

        var created = await createUseCase.ExecuteAsync("https://google.com");

        var urls = await getAllUseCase.ExecuteAsync();
        var urlList = urls.ToList();

        Assert.Single(urlList);
        Assert.Same(created, urlList[0]);
    }

    [Fact]
    public async Task IncrementAccessCount_WithExistingCode_ShouldIncrementAccessCount()
    {
        var repository = new FakeRepository();
        var codeGenerator = new FakeCodeGenerator("abc1234");
        var timeProvider = new FakeTimeProvider(
            new DateTimeOffset(2026, 6, 19, 12, 0, 0, TimeSpan.Zero)
        );

        var createUseCase = new CreateShortenedUrlUseCase(
            repository,
            codeGenerator,
            timeProvider
        );

        var created = await createUseCase.ExecuteAsync("https://google.com");

        var incremented = await repository.IncrementAccessCountAsync("abc1234");

        Assert.True(incremented);
        Assert.Equal(1, created.AccessCount);
    }

    [Fact]
    public async Task IncrementAccessCount_WithInvalidCode_ShouldReturnFalse()
    {
        var repository = new FakeRepository();

        var incremented = await repository.IncrementAccessCountAsync("invalid-code");

        Assert.False(incremented);
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

    public Task<bool> IncrementAccessCountAsync(
        string code, 
        CancellationToken cancellationToken = default)
    {
        if (!_urls.TryGetValue(code, out var shortenedUrl))
            return Task.FromResult(false);

        shortenedUrl.IncrementAccessCount();

        return Task.FromResult(true);
    }

    public Task<IEnumerable<ShortenedUrl>> GetAllCodesAsync(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_urls.Values.AsEnumerable());
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