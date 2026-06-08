using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pistachio.Api.Data;
using Pistachio.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Pistachio.Api.DTOs.Roles;

namespace Pistachio.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class RolesController : ControllerBase
{
    private readonly AppDbContext _context;

    public RolesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var roles = await _context.Roles
            .Select(r => new RoleResponse
            {
                Id = r.Id,
                Name = r.Name
            })
            .ToListAsync();

        return Ok(roles);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var role = await _context.Roles
            .Where(r => r.Id == id)
            .Select(r => new RoleResponse
            {
                Id = r.Id,
                Name = r.Name
            })
            .FirstOrDefaultAsync();

        if (role == null)
            return NotFound();

        return Ok(role);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateRoleRequest request)
    {
        var role = new Role
        {
            Name = request.Name
        };

        _context.Roles.Add(role);

        await _context.SaveChangesAsync();

        var response = new RoleResponse
        {
            Id = role.Id,
            Name = role.Name
        };

        return CreatedAtAction(nameof(GetById), new { id = role.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateRoleRequest request)
    {
        var role = await _context.Roles.FindAsync(id);

        if (role == null)
            return NotFound();

        role.Name = request.Name;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Roles.Any(r => r.Id == id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var role = await _context.Roles.FindAsync(id);

        if (role == null)
            return NotFound();

        _context.Roles.Remove(role);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}