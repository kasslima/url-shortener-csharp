using UrlShortener.Application.Abstractions;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Application.UseCases;

public class CreateShortenedUrlUseCase
{
    private const int MaximumAttempts = 5;

    private readonly IShortenedUrlRepository _repository;
    private readonly IShortCodeGenerator _codeGenerator;
    private readonly TimeProvider _timeProvider;

    public CreateShortenedUrlUseCase( IShortenedUrlRepository repository, IShortCodeGenerator codeGenerator, TimeProvider timeProvider){
        _repository = repository;
        _codeGenerator = codeGenerator;
        _timeProvider = timeProvider;
    }

    public ShortenedUrl Execute(string originalUrl)
    {
        for (var attempt = 0; attempt < MaximumAttempts; attempt++)
        {
            var code = _codeGenerator.Generate();

            var shortenedUrl = new ShortenedUrl(
                code,
                originalUrl,
                _timeProvider.GetUtcNow()
            );

            if (_repository.Add(shortenedUrl))
                return shortenedUrl;
        }

        throw new InvalidOperationException(
            "Could not generate a unique short code."
        );
    }
}