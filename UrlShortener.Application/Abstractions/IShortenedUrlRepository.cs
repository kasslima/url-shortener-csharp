using UrlShortener.Domain.Entities;

namespace UrlShortener.Application.Abstractions;

public interface IShortenedUrlRepository
{
    Task<bool> AddAsync( ShortenedUrl shortenedUrl, CancellationToken cancellationToken = default);

    Task<ShortenedUrl?> GetByCodeAsync( string code, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync( string code, CancellationToken cancellationToken = default);

    Task<bool> IncrementAccessCountAsync(string code, CancellationToken cancellationToken = default);

    Task<IEnumerable<ShortenedUrl>> GetAllCodesAsync(CancellationToken cancellationToken = default);
    
}