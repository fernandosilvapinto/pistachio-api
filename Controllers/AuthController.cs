using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Pistachio.Api.Data;
using Pistachio.Api.Models;
using Pistachio.Api.DTOs.Auth;
using Pistachio.Api.Services;


[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;
    private readonly IEmailService _emailService;

    public AuthController(AppDbContext context, IConfiguration config, IEmailService emailService)
    {
        _context = context;
        _config = config;
        _emailService = emailService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Credenciais inválidas" });
        }

        var token = GenerateJwtToken(user);

        return Ok(new LoginResponse
        {
            Token = token,
            Role = user.Role.Name,
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email
        });
    }

    // POST api/auth/register
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        // Verifica se o email já existe
        var exists = await _context.Users.AnyAsync(u => u.Email == request.Email);
        if (exists)
            return Conflict(new { message = "Este email já está registado." });

        // Procura o role Customer
        var customerRole = await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == "Customer");

        if (customerRole == null)
            return StatusCode(500, new { message = "Role Customer não encontrado." });

        // Cria o utilizador
        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            RoleId = customerRole.Id
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Conta criada com sucesso." });
    }

    // POST api/auth/forgot-password
    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        // Resposta genérica sempre igual, para não revelar se o email existe ou não.
        var genericResponse = Ok(new { message = "Se o email existir, vais receber instruções para repor a password." });

        if (user == null)
            return genericResponse;

        user.PasswordResetToken = TokenGenerator.GenerateUrlSafeToken();
        user.PasswordResetTokenExpiresAt = DateTime.UtcNow.AddHours(1);
        await _context.SaveChangesAsync();

        var clientUrl = _config["ClientUrl"] ?? "http://localhost:5174";
        var resetLink = $"{clientUrl}/reset-password?token={user.PasswordResetToken}";

        await _emailService.SendEmailAsync(
            user.Email,
            "Repor password — Pistachio",
            $"<p>Olá {user.Name},</p><p>Clica no link para definires uma nova password (válido durante 1 hora):</p><p><a href=\"{resetLink}\">{resetLink}</a></p>");

        return genericResponse;
    }

    // POST api/auth/reset-password
    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == request.Token);

        if (user == null || user.PasswordResetTokenExpiresAt == null || user.PasswordResetTokenExpiresAt < DateTime.UtcNow)
            return BadRequest(new { message = "Link inválido ou expirado. Pede um novo." });

        if (request.NewPassword.Length < 6)
            return BadRequest(new { message = "A password deve ter pelo menos 6 caracteres." });

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiresAt = null;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Password atualizada com sucesso." });
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _config.GetSection("Jwt");

        var keyValue = jwtSettings["Key"]
            ?? throw new InvalidOperationException("JWT Key is not configured.");
        var key = Encoding.UTF8.GetBytes(keyValue);

        var expiresInHoursValue = jwtSettings["ExpiresInHours"]
            ?? throw new InvalidOperationException("JWT ExpiresInHours is not configured.");
        var expires = DateTime.UtcNow.AddHours(double.Parse(expiresInHoursValue));


        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role.Name)
        };

        var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
