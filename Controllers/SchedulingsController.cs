using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pistachio.Api.Data;
using Pistachio.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Pistachio.Api.DTOs.Schedulings;
using Pistachio.Api.Services;
using Pistachio.Api.Authorization;
using Pistachio.Api.Identity;

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
        private readonly KeeperAdminClient _keeper;

        public SchedulingsController(
            AppDbContext context,
            IEmailService emailService,
            IConfiguration config,
            KeeperAdminClient keeper)
        {
            _context = context;
            _emailService = emailService;
            _config = config;
            _keeper = keeper;
        }

        // GET: api/schedulings/mine — só os agendamentos do utilizador autenticado (área do cliente)
        [HttpGet("mine")]
        [Authorize(Policy = Permissions.SchedulingRead)]
        public async Task<IActionResult> GetMine()
        {
            var localUserId = HttpContext.LocalUserId();
            if (localUserId is null)
                return Unauthorized();

            var userId = localUserId.Value;

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
                    AssigneeId = s.AssigneeId,
                    AssigneeName = s.Assignee != null ? s.Assignee.Name : null
                })
                .ToListAsync();

            return Ok(schedulings);
        }

        // GET: api/schedulings — todos os agendamentos (uso administrativo)
        [HttpGet]
        [Authorize(Policy = Permissions.SchedulingRead)]
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
                    AssigneeId = s.AssigneeId,
                    AssigneeName = s.Assignee != null ? s.Assignee.Name : null
                })
                .ToListAsync();

            return Ok(schedulings);
        }

        // GET: api/schedulings/{id}
        [HttpGet("{id}")]
        [Authorize(Policy = Permissions.SchedulingRead)]
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
                    AssigneeId = s.AssigneeId,
                    AssigneeName = s.Assignee != null ? s.Assignee.Name : null
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
            return NotFound();
            // var service = await _context.Services.FindAsync(request.ServiceId);
            // if (service == null || !service.IsActive)
            //     return BadRequest(new { message = "Serviço inválido." });

            // var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            // var isNewAccount = user == null;
            // var signInUrl = _config["ClientUrl"] ?? "http://localhost:5174";

            // if (user == null)
            // {
            //     // Referência local sem identidade. Subject fica vazio até a pessoa
            //     // se autenticar no Keeper com este email, momento em que a linha
            //     // é reclamada pelo aprovisionamento just-in-time.
            //     user = new User
            //     {
            //         Name = request.Name,
            //         Email = request.Email
            //     };

            //     _context.Users.Add(user);
            //     await _context.SaveChangesAsync();
            // }

            // var scheduling = new Scheduling
            // {
            //     ScheduledDate = request.ScheduledDate,
            //     ServiceName = service.Name,
            //     UserId = user.Id,
            //     ServiceId = service.Id,
            // };

            // _context.Schedulings.Add(scheduling);
            // await _context.SaveChangesAsync();

            // // Convida a pessoa a criar conta no identity provider. A password
            // // é definida lá, nunca aqui. Uma falha no convite não invalida a
            // // marcação, que já está gravada.
            // var invitation = await _keeper.InviteCustomerAsync(
            //     request.Email,
            //     request.Name,
            //     HttpContext.RequestAborted);

            // var confirmation =
            //     $"<p>Olá {user.Name},</p>" +
            //     $"<p>O teu agendamento para <strong>{service.Name}</strong> em {request.ScheduledDate:dd/MM/yyyy HH:mm} foi confirmado.</p>";

            // var emailBody = invitation switch
            // {
            //     CustomerInvitationResult.Invited =>
            //         confirmation +
            //         "<p>Enviámos-te noutra mensagem um link para definires a tua password e acompanhares os teus agendamentos.</p>",

            //     CustomerInvitationResult.AlreadyRegistered =>
            //         confirmation +
            //         $"<p>Já tens conta connosco — inicia sessão em <a href=\"{signInUrl}\">{signInUrl}</a> para veres os detalhes.</p>",

            //     _ =>
            //         confirmation +
            //         $"<p>Podes acompanhar os teus agendamentos em <a href=\"{signInUrl}\">{signInUrl}</a>.</p>"
            // };

            // await _emailService.SendEmailAsync(user.Email, "Agendamento confirmado — Pistachio", emailBody);

            // return Ok(new
            // {
            //     message = "Agendamento criado com sucesso.",
            //     isNewAccount,
            //     invitation = invitation.ToString(),
            //     schedulingId = scheduling.Id,
            // });
        }

        [HttpPost]
        [Authorize(Policy = Permissions.SchedulingWrite)]
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
                AssigneeId = scheduling.AssigneeId
            };

            return CreatedAtAction(nameof(GetById), new { id = scheduling.Id }, response);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = Permissions.SchedulingWrite)]
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
        [Authorize(Policy = Permissions.SchedulingStatus)]
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

        // PATCH: api/schedulings/{id}/assignee
        [HttpPatch("{id}/assignee")]
        [Authorize(Policy = Permissions.SchedulingAssign)]
        public async Task<IActionResult> UpdateAssignee(int id, UpdateSchedulingAssigneeRequest request)
        {
            var scheduling = await _context.Schedulings.FindAsync(id);

            if (scheduling == null)
                return NotFound();

            if (request.AssigneeId.HasValue)
            {
                var assignee = await _context.Users.FindAsync(request.AssigneeId.Value);
                if (assignee == null)
                    return BadRequest("Utilizador não encontrado.");
            }

            scheduling.AssigneeId = request.AssigneeId;

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
                    AssigneeId = s.AssigneeId,
                    AssigneeName = s.Assignee != null ? s.Assignee.Name : null
                })
                .FirstOrDefaultAsync();

            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = Permissions.SchedulingDelete)]
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
