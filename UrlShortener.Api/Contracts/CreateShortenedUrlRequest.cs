namespace UrlShortener.Api.Contracts;

using System.ComponentModel.DataAnnotations;

public record CreateShortenedUrlRequest(
    [param: Required(ErrorMessage = "The URL is required.")]
    [param: Url(ErrorMessage = "Please provide a valid URL.")]
    string Url
);
