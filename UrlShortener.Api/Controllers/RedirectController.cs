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
    public IActionResult RedirectToOriginal(string code)
    {
        var shortenedUrl = _getUseCase.Execute(code);

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