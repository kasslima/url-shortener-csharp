using UrlShortener.Domain.Entities;

namespace UrlShortener.Tests.Domain;

public class ShortenedUrlTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateShortenedUrl()
    {
        // Arrange
        const string code = "aB12cD";
        const string originalUrl = "https://google.com";
        var createdAt = DateTimeOffset.UtcNow;

        // Act
        var shortenedUrl = new ShortenedUrl(
            code,
            originalUrl,
            createdAt
        );

        // Assert
        Assert.Equal(code, shortenedUrl.Code);
        Assert.Equal(originalUrl, shortenedUrl.OriginalUrl);
        Assert.Equal(createdAt, shortenedUrl.CreatedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("google.com")]
    [InlineData("ftp://google.com")]
    [InlineData("texto qualquer")]
    public void Constructor_WithInvalidUrl_ShouldThrowException(string invalidUrl)
    {
        // Act
        var action = () => new ShortenedUrl(
            "aB12cD",
            invalidUrl,
            DateTimeOffset.UtcNow
        );

        // Assert
        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Constructor_WithEmptyCode_ShouldThrowException()
    {
        var action = () => new ShortenedUrl(
            "",
            "https://google.com",
            DateTimeOffset.UtcNow
        );

        Assert.Throws<ArgumentException>(action);
    }
}