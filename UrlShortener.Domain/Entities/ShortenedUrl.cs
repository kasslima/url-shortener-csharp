namespace UrlShortener.Domain.Entities;

public class ShortenedUrl
{
    public string Code { get; }
    public string OriginalUrl { get; }
    public DateTimeOffset CreatedAt { get; }

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
        return Uri.TryCreate(url, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}