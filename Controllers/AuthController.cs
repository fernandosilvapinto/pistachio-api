using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pistachio.Api.Data;
using Pistachio.Api.Identity;

namespace Pistachio.Api.Controllers;

/// <summary>
/// Authentication happens in Keeper. This controller only reports what the
/// current token grants, so the front end knows what to render.
/// </summary>
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var localUserId = HttpContext.LocalUserId();

        if (localUserId is null)
            return Unauthorized();

        var user = await _context.Users
            .Where(u => u.Id == localUserId)
            .Select(u => new UserResponse
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                HasSignedIn = u.Subject != string.Empty,
                CreatedAt = u.CreatedAt,
                LastSeenAt = u.LastSeenAt
            })
            .FirstOrDefaultAsync();

        if (user is null)
            return NotFound();

        return Ok(new
        {
            user,
            subject = User.FindFirst("sub")?.Value,
            permissions = ReadPermissions()
        });
    }

    private IReadOnlyCollection<string> ReadPermissions()
    {
        var audience = _configuration["Keeper:Audience"];
        var raw = User.FindFirst("resource_access")?.Value;

        if (string.IsNullOrEmpty(audience) || string.IsNullOrEmpty(raw))
            return [];

        using var document = JsonDocument.Parse(raw);

        if (!document.RootElement.TryGetProperty(audience, out var resource) ||
            !resource.TryGetProperty("roles", out var roles))
        {
            return [];
        }

        return roles.EnumerateArray()
            .Select(role => role.GetString())
            .Where(role => role is not null)
            .Select(role => role!)
            .ToArray();
    }
}
