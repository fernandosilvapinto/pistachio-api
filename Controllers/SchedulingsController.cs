using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pistachio.Api.Data;
using Pistachio.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Pistachio.Api.DTOs.Schedulings;

namespace Pistachio.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SchedulingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SchedulingsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/schedulings
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var schedulings = await _context.Schedulings
                .Select(s => new SchedulingResponse
                {
                    Id = s.Id,
                    ScheduledDate = s.ScheduledDate,
                    ServiceName = s.ServiceName,
                    UserId = s.UserId,
                    ServiceId = s.ServiceId
                })
                .ToListAsync();

            return Ok(schedulings);
        }

        // GET: api/schedulings/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var scheduling = await _context.Schedulings
                .Where(s => s.Id == id)
                .Select(s => new SchedulingResponse
                {
                    Id = s.Id,
                    ScheduledDate = s.ScheduledDate,
                    ServiceName = s.ServiceName,
                    UserId = s.UserId,
                    ServiceId = s.ServiceId
                })
                .FirstOrDefaultAsync();

            if (scheduling == null)
                return NotFound();

            return Ok(scheduling);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSchedulingRequest request)
        {
            var scheduling = new Scheduling
            {
                ScheduledDate = request.ScheduledDate,
                ServiceName = request.ServiceName,
                UserId = request.UserId,
                ServiceId = request.ServiceId
            };

            _context.Schedulings.Add(scheduling);

            await _context.SaveChangesAsync();

            var response = new SchedulingResponse
            {
                Id = scheduling.Id,
                ScheduledDate = scheduling.ScheduledDate,
                ServiceName = scheduling.ServiceName,
                UserId = scheduling.UserId,
                ServiceId = scheduling.ServiceId
            };

            return CreatedAtAction(nameof(GetById), new { id = scheduling.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateSchedulingRequest request)
        {
            var scheduling = await _context.Schedulings.FindAsync(id);

            if (scheduling == null)
                return NotFound();

            scheduling.ScheduledDate = request.ScheduledDate;
            scheduling.ServiceName = request.ServiceName;
            scheduling.UserId = request.UserId;
            scheduling.ServiceId = request.ServiceId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Schedulings.Any(s => s.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var scheduling = await _context.Schedulings.FindAsync(id);
            if (scheduling == null) return NotFound();

            _context.Schedulings.Remove(scheduling);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
