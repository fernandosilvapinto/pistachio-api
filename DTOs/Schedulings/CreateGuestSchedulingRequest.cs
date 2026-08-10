namespace Pistachio.Api.DTOs.Schedulings;

public class CreateGuestSchedulingRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int ServiceId { get; set; }
    public DateTime ScheduledDate { get; set; }
}
