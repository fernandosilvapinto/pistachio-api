using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pistachio.Api.Data;
using Pistachio.Api.Models;
using Pistachio.Api.DTOs.Users;
using System.Security.Claims;

namespace Pistachio.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UsersController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .Select(u => new UserResponse
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role.Name,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.Id == id)
                .Select(u => new UserResponse
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role.Name,
                    CreatedAt = u.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // POST api/users
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserRequest request)
        {
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                RoleId = request.RoleId
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            user = await _context.Users
                .Include(u => u.Role)
                .FirstAsync(u => u.Id == user.Id);

            var response = new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.Name,
                CreatedAt = user.CreatedAt
            };

            return CreatedAtAction(
                nameof(GetById),
                new { id = user.Id },
                response);
        }


        // PUT api/users/5
        [HttpPut("{id}")]
        //[HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateUserRequest request)
        {
            var existingUser = await _context.Users.FindAsync(id);

            if (existingUser == null)
                return NotFound();

            existingUser.Name = request.Name;
            existingUser.Email = request.Email;
            existingUser.RoleId = request.RoleId;

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                existingUser.PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(request.Password);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }


        // DELETE api/users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var user = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.Email == email)
                .Select(u => new UserResponse
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role.Name,
                    CreatedAt = u.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound();

            return Ok(user);
        }
    }


}
