namespace UrlShortener.Domain.Entities;

public class ShortenedUrl
{
    public string Code { get; private set; } = string.Empty;
    public string OriginalUrl { get; private set; } = string.Empty;
    public int AccessCount { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private ShortenedUrl()
    {
        // EF Core
    }

    public ShortenedUrl( string code, string originalUrl, DateTimeOffset createdAt)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code is required.");

        if (!IsValidUrl(originalUrl))
            throw new ArgumentException("A valid URL is required.");

        Code = code;
        OriginalUrl = originalUrl;
        CreatedAt = createdAt;
    }

    private static bool IsValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri)
               && (uri.Scheme == Uri.UriSchemeHttp
                   || uri.Scheme == Uri.UriSchemeHttps);
    }

    public void IncrementAccessCount()
    {
        AccessCount++;
    }
}