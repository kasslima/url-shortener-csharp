namespace UrlShortener.Api.Contracts;

public record GetAllShortenedUrlResponse(
    string Code,
    string OriginalUrl,
    string ShortUrl,
    int AccessCount,
    DateTimeOffset CreatedAt
);