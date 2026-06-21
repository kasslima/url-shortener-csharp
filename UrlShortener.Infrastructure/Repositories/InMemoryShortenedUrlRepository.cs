using System.Collections.Concurrent;
using UrlShortener.Application.Abstractions;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure.Repositories;

public class InMemoryShortenedUrlRepository : IShortenedUrlRepository
{
    private readonly ConcurrentDictionary<string, ShortenedUrl> _urls = new();

    public Task<bool> AddAsync( ShortenedUrl shortenedUrl, CancellationToken cancellationToken = default)
    {
        var added = _urls.TryAdd(shortenedUrl.Code, shortenedUrl);

        return Task.FromResult(added);
    }

    public Task<ShortenedUrl?> GetByCodeAsync( string code, CancellationToken cancellationToken = default)
    {
        _urls.TryGetValue(code, out var shortenedUrl);

        return Task.FromResult(shortenedUrl);
    }

    public Task<bool> DeleteAsync( string code, CancellationToken cancellationToken = default)  
    {
        var deleted = _urls.TryRemove(code, out _);

        return Task.FromResult(deleted);
    }

    public Task<bool> IncrementAccessCountAsync( string code, CancellationToken cancellationToken = default)
    {
        if (!_urls.TryGetValue(code, out var shortenedUrl))
            return Task.FromResult(false);

        shortenedUrl.IncrementAccessCount();

        return Task.FromResult(true);
    }

    public Task<IEnumerable<ShortenedUrl>> GetAllCodesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_urls.Values.AsEnumerable());
    }
}