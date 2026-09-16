using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pistachio.Api.Authorization;
using Pistachio.Api.Data;

namespace Pistachio.Api.Controllers
{
    /// <summary>
    /// Read-only view over the local reference rows. Creating, disabling and
    /// renaming people are identity operations and happen in Anvil.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context) => _context = context;

        [HttpGet]
        [Authorize(Policy = Permissions.UsersRead)]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users
                .OrderBy(u => u.Name)
                .Select(u => new UserResponse
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    HasSignedIn = u.Subject != string.Empty,
                    CreatedAt = u.CreatedAt,
                    LastSeenAt = u.LastSeenAt
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = Permissions.UsersRead)]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _context.Users
                .Where(u => u.Id == id)
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

            if (user == null)
                return NotFound();

            return Ok(user);
        }
    }
}
