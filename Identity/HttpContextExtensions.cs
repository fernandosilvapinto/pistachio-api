namespace Pistachio.Api.Identity;

public static class HttpContextExtensions
{
    /// <summary>
    /// Identifier of the local reference row for the authenticated identity,
    /// set by <see cref="UserProvisioningMiddleware"/>.
    /// </summary>
    public static int? LocalUserId(this HttpContext context)
        => context.Items.TryGetValue(UserProvisioningMiddleware.LocalUserIdKey, out var value)
           && value is int id
            ? id
            : null;
}
