using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pistachio.Api.Data;
using Pistachio.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Pistachio.Api.DTOs.Services;

namespace Pistachio.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ServicesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Services
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var services = await _context.Services
                .Select(s => new ServiceResponse
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    Price = s.Price,
                    IsActive = s.IsActive,
                    IsFeatured = s.IsFeatured
                })
                .ToListAsync();

            return Ok(services);
        }

        // GET: api/Services/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var service = await _context.Services
                .Where(s => s.Id == id)
                .Select(s => new ServiceResponse
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    Price = s.Price,
                    IsActive = s.IsActive,
                    IsFeatured = s.IsFeatured
                })
                .FirstOrDefaultAsync();

            if (service == null)
                return NotFound();

            return Ok(service);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateServiceRequest request)
        {
            var service = new Service
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                IsActive = request.IsActive,
                IsFeatured = request.IsFeatured
            };

            _context.Services.Add(service);

            await _context.SaveChangesAsync();

            var response = new ServiceResponse
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Price = service.Price,
                IsActive = service.IsActive,
                IsFeatured = service.IsFeatured
            };

            return CreatedAtAction(nameof(GetById), new { id = service.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateServiceRequest request)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null)
                return NotFound();

            service.Name = request.Name;
            service.Description = request.Description;
            service.Price = request.Price;
            service.IsActive = request.IsActive;
            service.IsFeatured = request.IsFeatured;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Services.Any(s => s.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null) return NotFound();

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // Listar apenas destaques? 
        // Criar a logica para trabalhar os serviços em "destaque" ?
        // [HttpGet("destaques")]
        // public async Task<IActionResult> GetDestaques()
        // {
        //     var destaques = await _context.Services
        //         .Where(s => s.Destaque)
        //         .ToListAsync();

        //     return Ok(destaques);
        // }

    }
}
