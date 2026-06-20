using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.UseCases;

namespace UrlShortener.Api.Controllers;

[ApiController]
public class RedirectController : ControllerBase
{
    private readonly GetShortenedUrlUseCase _getUseCase;

    public RedirectController(GetShortenedUrlUseCase getUseCase)
    {
        _getUseCase = getUseCase;
    }

    [HttpGet("/{code}")]
    public async Task<IActionResult> RedirectToOriginal(
        string code,
        CancellationToken cancellationToken)
    {
        var shortenedUrl = await _getUseCase.ExecuteAsync(
            code,
            cancellationToken
        );

        if (shortenedUrl is null)
        {
            return NotFound(new
            {
                message = "Short URL not found."
            });
        }

        return Redirect(shortenedUrl.OriginalUrl);
    }
}