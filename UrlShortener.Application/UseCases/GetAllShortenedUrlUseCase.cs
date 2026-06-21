using UrlShortener.Application.Abstractions;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Application.UseCases;

public class GetAllShortenedUrlUseCase
{
    private readonly IShortenedUrlRepository _repository;

    public GetAllShortenedUrlUseCase(IShortenedUrlRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ShortenedUrl>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return await _repository.GetAllCodesAsync(cancellationToken);
    }
}