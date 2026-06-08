namespace Pistachio.Api.DTOs.Users;

public class UpdateUserRequest
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Password { get; set; }

    public int RoleId { get; set; }
}