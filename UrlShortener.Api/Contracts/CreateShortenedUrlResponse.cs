namespace UrlShortener.Api.Contracts;

public record CreateShortenedUrlResponse(
    string Code,
    string OriginalUrl,
    string ShortUrl,
    DateTimeOffset CreatedAt
);