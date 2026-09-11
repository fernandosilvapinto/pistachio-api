using Microsoft.IdentityModel.JsonWebTokens;

namespace Pistachio.Api.Identity;

/// <summary>
/// This API serves two populations from two realms: the operator's own staff
/// and its customers. Their tokens are signed by different issuers, so the
/// bearer scheme is chosen per request by reading the unvalidated issuer and
/// then handing the token to the scheme that knows how to verify it.
/// </summary>
public static class IssuerSchemes
{
    public const string Selector = "Keeper";
    public const string Workforce = "Keeper.Workforce";
    public const string Customers = "Keeper.Customers";

    private static readonly JsonWebTokenHandler Handler = new();

    public static string Select(HttpContext context, string customersAuthority)
    {
        var header = context.Request.Headers.Authorization.ToString();

        if (!header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return Workforce;
        }

        var token = header["Bearer ".Length..].Trim();

        if (!Handler.CanReadToken(token))
        {
            return Workforce;
        }

        try
        {
            var issuer = Handler.ReadJsonWebToken(token).Issuer;

            return string.Equals(issuer, customersAuthority.TrimEnd('/'), StringComparison.Ordinal)
                ? Customers
                : Workforce;
        }
        catch
        {
            // A malformed token is not this method's problem: hand it to a real
            // scheme and let validation reject it with a proper 401.
            return Workforce;
        }
    }
}
