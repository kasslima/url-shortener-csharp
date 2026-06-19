using System.Collections.Concurrent;
using UrlShortener.Application.Abstractions;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure.Repositories;

public class InMemoryShortenedUrlRepository : IShortenedUrlRepository
{
    private readonly ConcurrentDictionary<string, ShortenedUrl> _urls = new();

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
        return _urls.TryRemove(code, out _);
    }
}