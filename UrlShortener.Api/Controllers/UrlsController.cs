using Microsoft.AspNetCore.Mvc;
using UrlShortener.Api.Contracts;
using UrlShortener.Application.UseCases;

namespace UrlShortener.Api.Controllers;

[ApiController]
[Route("api/urls")]
public class UrlsController : ControllerBase
{
    private readonly CreateShortenedUrlUseCase _createUseCase;
    private readonly GetAllShortenedUrlUseCase _getAllUseCase;

    public UrlsController(CreateShortenedUrlUseCase createUseCase, GetAllShortenedUrlUseCase getAllUseCase)
    {
        _createUseCase = createUseCase;
        _getAllUseCase = getAllUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateShortenedUrlRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var shortenedUrl = await _createUseCase.ExecuteAsync(
                request.Url,
                cancellationToken
            );

            var shortUrl =
                $"{Request.Scheme}://{Request.Host}/{shortenedUrl.Code}";

            var response = new CreateShortenedUrlResponse(
                shortenedUrl.Code,
                shortenedUrl.OriginalUrl,
                shortUrl,
                shortenedUrl.AccessCount,
                shortenedUrl.CreatedAt
            );

            return StatusCode(StatusCodes.Status201Created, response);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var shortenedUrls = await _getAllUseCase.ExecuteAsync(cancellationToken);

            var response = shortenedUrls.Select(x => new GetAllShortenedUrlResponse(
                x.Code,
                x.OriginalUrl,
                $"{Request.Scheme}://{Request.Host}/{x.Code}",
                x.AccessCount,
                x.CreatedAt
            ));

            return Ok(response);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }
}