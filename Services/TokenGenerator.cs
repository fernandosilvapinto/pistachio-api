using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;

namespace Pistachio.Api.Services;

public static class TokenGenerator
{
    // Token aleatório, seguro e seguro para colocar num URL (usado em links de email)
    public static string GenerateUrlSafeToken(int bytesLength = 32)
    {
        var bytes = RandomNumberGenerator.GetBytes(bytesLength);
        return WebEncoders.Base64UrlEncode(bytes);
    }
}
