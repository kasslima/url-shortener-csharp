using UrlShortener.Application.Abstractions;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Application.UseCases;

public class GetShortenedUrlUseCase
{
    private readonly IShortenedUrlRepository _repository;

    public GetShortenedUrlUseCase(
        IShortenedUrlRepository repository)
    {
        _repository = repository;
    }

    public ShortenedUrl? Execute(string code)
    {
        return _repository.GetByCode(code);
    }
}