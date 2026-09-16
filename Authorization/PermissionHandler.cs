using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace Pistachio.Api.Authorization;

public sealed class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private const string ResourceAccessClaim = "resource_access";

    private readonly string _audience;

    public PermissionHandler(IConfiguration configuration)
        => _audience = configuration["Anvil:Audience"]
           ?? throw new InvalidOperationException("Anvil:Audience is not configured.");

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var raw = context.User.FindFirst(ResourceAccessClaim)?.Value;

        if (string.IsNullOrEmpty(raw))
        {
            return Task.CompletedTask;
        }

        using var document = JsonDocument.Parse(raw);

        if (document.RootElement.TryGetProperty(_audience, out var resource) &&
            resource.TryGetProperty("roles", out var roles) &&
            roles.EnumerateArray().Any(role => role.GetString() == requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
