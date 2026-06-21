using UrlShortener.Application.Abstractions;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Application.UseCases;

public class GetShortenedUrlUseCase
{
    private readonly IShortenedUrlRepository _repository;

    public GetShortenedUrlUseCase(IShortenedUrlRepository repository)
    {
        _repository = repository;
    }

    public async Task<ShortenedUrl?> ExecuteAsync( string code, CancellationToken cancellationToken = default)
    {
       var shortenedUrl = await _repository.GetByCodeAsync(code, cancellationToken);

       if (shortenedUrl is null)
            return null;

        await _repository.IncrementAccessCountAsync(code, cancellationToken);

        return shortenedUrl;
    }
}