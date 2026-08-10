using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pistachio.Api.Data;
using Pistachio.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Pistachio.Api.DTOs.Schedulings;
using Pistachio.Api.Services;
using System.Security.Claims;

namespace Pistachio.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SchedulingsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;

        public SchedulingsController(AppDbContext context, IEmailService emailService, IConfiguration config)
        {
            _context = context;
            _emailService = emailService;
            _config = config;
        }

        // GET: api/schedulings/mine — só os agendamentos do utilizador autenticado (área do cliente)
        [HttpGet("mine")]
        public async Task<IActionResult> GetMine()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var schedulings = await _context.Schedulings
                .Where(s => s.UserId == userId)
                .Select(s => new SchedulingResponse
                {
                    Id = s.Id,
                    ScheduledDate = s.ScheduledDate,
                    ServiceName = s.ServiceName,
                    Status = s.Status,
                    UserId = s.UserId,
                    UserName = s.User.Name,
                    ServiceId = s.ServiceId,
                    ServiceDescription = s.Service != null ? s.Service.Description : string.Empty,
                    AssignedMechanicId = s.AssignedMechanicId,
                    AssignedMechanicName = s.AssignedMechanic != null ? s.AssignedMechanic.Name : null
                })
                .ToListAsync();

            return Ok(schedulings);
        }

        // GET: api/schedulings — todos os agendamentos (uso administrativo)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var schedulings = await _context.Schedulings
                .Select(s => new SchedulingResponse
                {
                    Id = s.Id,
                    ScheduledDate = s.ScheduledDate,
                    ServiceName = s.ServiceName,
                    Status = s.Status,
                    UserId = s.UserId,
                    UserName = s.User.Name,
                    ServiceId = s.ServiceId,
                    ServiceDescription = s.Service != null ? s.Service.Description : string.Empty,
                    AssignedMechanicId = s.AssignedMechanicId,
                    AssignedMechanicName = s.AssignedMechanic != null ? s.AssignedMechanic.Name : null
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
                    Status = s.Status,
                    UserId = s.UserId,
                    UserName = s.User.Name,
                    ServiceId = s.ServiceId,
                    ServiceDescription = s.Service != null ? s.Service.Description : string.Empty,
                    AssignedMechanicId = s.AssignedMechanicId,
                    AssignedMechanicName = s.AssignedMechanic != null ? s.AssignedMechanic.Name : null
                })
                .FirstOrDefaultAsync();

            if (scheduling == null)
                return NotFound();

            return Ok(scheduling);
        }

        // POST: api/schedulings/guest — agendamento sem login prévio
        [AllowAnonymous]
        [HttpPost("guest")]
        public async Task<IActionResult> CreateGuest(CreateGuestSchedulingRequest request)
        {
            var service = await _context.Services.FindAsync(request.ServiceId);
            if (service == null || !service.IsActive)
                return BadRequest(new { message = "Serviço inválido." });

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            var isNewAccount = user == null;
            string? resetLink = null;

            if (user == null)
            {
                var customerRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Customer");
                if (customerRole == null)
                    return StatusCode(500, new { message = "Role Customer não encontrado." });

                var resetToken = TokenGenerator.GenerateUrlSafeToken();

                user = new User
                {
                    Name = request.Name,
                    Email = request.Email,
                    // Password ainda não definida — a hash aqui é só um valor não utilizável até o reset.
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(TokenGenerator.GenerateUrlSafeToken()),
                    RoleId = customerRole.Id,
                    PasswordResetToken = resetToken,
                    PasswordResetTokenExpiresAt = DateTime.UtcNow.AddHours(24),
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var clientUrl = _config["ClientUrl"] ?? "http://localhost:5174";
                resetLink = $"{clientUrl}/reset-password?token={resetToken}";
            }

            var scheduling = new Scheduling
            {
                ScheduledDate = request.ScheduledDate,
                ServiceName = service.Name,
                UserId = user.Id,
                ServiceId = service.Id,
            };

            _context.Schedulings.Add(scheduling);
            await _context.SaveChangesAsync();

            var emailBody = isNewAccount
                ? $"<p>Olá {user.Name},</p>" +
                  $"<p>O teu agendamento para <strong>{service.Name}</strong> em {request.ScheduledDate:dd/MM/yyyy HH:mm} foi confirmado.</p>" +
                  $"<p>Criámos uma conta para acompanhares os teus agendamentos. Define a tua password aqui:</p>" +
                  $"<p><a href=\"{resetLink}\">{resetLink}</a></p>"
                : $"<p>Olá {user.Name},</p>" +
                  $"<p>O teu agendamento para <strong>{service.Name}</strong> em {request.ScheduledDate:dd/MM/yyyy HH:mm} foi confirmado.</p>" +
                  $"<p>Já tens conta connosco — inicia sessão para veres os detalhes.</p>";

            await _emailService.SendEmailAsync(user.Email, "Agendamento confirmado — Pistachio", emailBody);

            return Ok(new
            {
                message = "Agendamento criado com sucesso.",
                isNewAccount,
                schedulingId = scheduling.Id,
            });
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
                Status = scheduling.Status,
                UserId = scheduling.UserId,
                ServiceId = scheduling.ServiceId,
                AssignedMechanicId = scheduling.AssignedMechanicId
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
        
        // PATCH: api/schedulings/{id}/status
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateSchedulingStatusRequest request)
        {
            var scheduling = await _context.Schedulings.FindAsync(id);

            if (scheduling == null)
                return NotFound();

            scheduling.Status = request.Status;

            await _context.SaveChangesAsync();

            var response = new SchedulingResponse
            {
                Id = scheduling.Id,
                ScheduledDate = scheduling.ScheduledDate,
                ServiceName = scheduling.ServiceName,
                Status = scheduling.Status,
                UserId = scheduling.UserId,
                ServiceId = scheduling.ServiceId
            };

            return Ok(response);
        }

        // PATCH: api/schedulings/{id}/mechanic
        [HttpPatch("{id}/mechanic")]
        public async Task<IActionResult> UpdateMechanic(int id, UpdateSchedulingMechanicRequest request)
        {
            var scheduling = await _context.Schedulings.FindAsync(id);

            if (scheduling == null)
                return NotFound();

            if (request.AssignedMechanicId.HasValue)
            {
                var mechanic = await _context.Users.FindAsync(request.AssignedMechanicId.Value);
                if (mechanic == null)
                    return BadRequest("Mecânico não encontrado.");
            }

            scheduling.AssignedMechanicId = request.AssignedMechanicId;

            await _context.SaveChangesAsync();

            var response = await _context.Schedulings
                .Where(s => s.Id == id)
                .Select(s => new SchedulingResponse
                {
                    Id = s.Id,
                    ScheduledDate = s.ScheduledDate,
                    ServiceName = s.ServiceName,
                    Status = s.Status,
                    UserId = s.UserId,
                    UserName = s.User.Name,
                    ServiceId = s.ServiceId,
                    ServiceDescription = s.Service != null ? s.Service.Description : string.Empty,
                    AssignedMechanicId = s.AssignedMechanicId,
                    AssignedMechanicName = s.AssignedMechanic != null ? s.AssignedMechanic.Name : null
                })
                .FirstOrDefaultAsync();

            return Ok(response);
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
