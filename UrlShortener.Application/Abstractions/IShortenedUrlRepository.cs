using UrlShortener.Domain.Entities;

namespace UrlShortener.Application.Abstractions;

public interface IShortenedUrlRepository
{
    bool Add(ShortenedUrl shortenedUrl);
    ShortenedUrl? GetByCode(string code);
    bool Delete(string code);
}