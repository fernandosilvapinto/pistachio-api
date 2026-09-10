namespace Pistachio.Api.Identity;

public sealed class UserProvisioningMiddleware
{
    public const string LocalUserIdKey = "LocalUserId";

    private readonly RequestDelegate _next;

    public UserProvisioningMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, UserProvisioning provisioning)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var user = await provisioning.EnsureAsync(context.User, context.RequestAborted);
            context.Items[LocalUserIdKey] = user.Id;
        }

        await _next(context);
    }
}
