using Microsoft.EntityFrameworkCore;
using UrlShortener.Application.Abstractions;
using UrlShortener.Domain.Entities;
using UrlShortener.Infrastructure.Persistence;

namespace UrlShortener.Infrastructure.Repositories;

public class PostgresShortenedUrlRepository : IShortenedUrlRepository
{
    private readonly UrlShortenerDbContext _dbContext;

    public PostgresShortenedUrlRepository(
        UrlShortenerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> AddAsync(
        ShortenedUrl shortenedUrl,
        CancellationToken cancellationToken = default)
    {
        var exists = await _dbContext.ShortenedUrls
            .AnyAsync(x => x.Code == shortenedUrl.Code, cancellationToken);

        if (exists)
            return false;

        _dbContext.ShortenedUrls.Add(shortenedUrl);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<ShortenedUrl?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ShortenedUrls
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
    }

    public async Task<bool> IncrementAccessCountAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var shortenedUrl = await _dbContext.ShortenedUrls
            .FirstOrDefaultAsync(x => x.Code == code, cancellationToken);

        if (shortenedUrl is null)
            return false;

        shortenedUrl.IncrementAccessCount();

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var shortenedUrl = await _dbContext.ShortenedUrls
            .FirstOrDefaultAsync(x => x.Code == code, cancellationToken);

        if (shortenedUrl is null)
            return false;

        _dbContext.ShortenedUrls.Remove(shortenedUrl);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IEnumerable<ShortenedUrl>> GetAllCodesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.ShortenedUrls
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}