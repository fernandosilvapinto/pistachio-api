using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Pistachio.Api.Data;
using Pistachio.Api.Models;

namespace Pistachio.Api.Identity;

/// <summary>
/// Keeps a local reference row for every identity that reaches this API.
/// Identity itself lives in Anvil; this row only exists so domain records
/// can point at a person.
/// </summary>
public sealed class UserProvisioning
{
    private readonly AppDbContext _context;

    public UserProvisioning(AppDbContext context) => _context = context;

    public async Task<User> EnsureAsync(ClaimsPrincipal principal, CancellationToken ct)
    {
        var subject = principal.FindFirst("sub")?.Value
            ?? throw new InvalidOperationException("Token has no subject.");

        var name = principal.FindFirst("name")?.Value ?? string.Empty;
        var email = principal.FindFirst("email")?.Value ?? string.Empty;
        var emailVerified = principal.FindFirst("email_verified")?.Value == "true";

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Subject == subject, ct);

        if (user is null && emailVerified && !string.IsNullOrEmpty(email))
        {
            user = await _context.Users
                .FirstOrDefaultAsync(u => u.Subject == string.Empty && u.Email == email, ct);

            if (user is not null)
            {
                user.Subject = subject;
            }
        }

        if (user is null)
        {
            user = new User { Subject = subject };
            _context.Users.Add(user);
        }

        user.Name = name;
        user.Email = email;
        user.LastSeenAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return user;
    }
}
