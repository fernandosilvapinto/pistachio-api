namespace Pistachio.Api.DTOs.Services;

public class CreateServiceRequest
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsFeatured { get; set; } = false;
}