namespace Pistachio.Api.DTOs.Services;

public class ServiceResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public bool IsActive { get; set; }

    public bool IsFeatured { get; set; }
}