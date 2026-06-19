using Microsoft.AspNetCore.Mvc;
using UrlShortener.Api.Contracts;
using UrlShortener.Application.UseCases;

namespace UrlShortener.Api.Controllers;

[ApiController]
[Route("api/urls")]
public class UrlsController : ControllerBase
{
    private readonly CreateShortenedUrlUseCase _createUseCase;

    public UrlsController(CreateShortenedUrlUseCase createUseCase)
    {
        _createUseCase = createUseCase;
    }

    [HttpPost]
    public IActionResult Create(CreateShortenedUrlRequest request)
    {
        try
        {
            var shortenedUrl = _createUseCase.Execute(request.Url);

            var shortUrl =
                $"{Request.Scheme}://{Request.Host}/{shortenedUrl.Code}";

            var response = new CreateShortenedUrlResponse(
                shortenedUrl.Code,
                shortenedUrl.OriginalUrl,
                shortUrl,
                shortenedUrl.CreatedAt
            );

            return StatusCode(StatusCodes.Status201Created, response);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }
}