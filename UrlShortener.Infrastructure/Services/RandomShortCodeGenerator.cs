using System.Security.Cryptography;
using UrlShortener.Application.Abstractions;

namespace UrlShortener.Infrastructure.Services;

public class RandomShortCodeGenerator : IShortCodeGenerator
{
    private const string Characters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    private const int CodeLength = 7;

    public string Generate()
    {
        return RandomNumberGenerator.GetString(
            Characters,
            CodeLength
        );
    }
}