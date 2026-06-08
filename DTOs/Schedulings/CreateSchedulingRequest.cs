namespace Pistachio.Api.DTOs.Schedulings;

public class CreateSchedulingRequest
{
    public DateTime ScheduledDate { get; set; }

    public string ServiceName { get; set; } = string.Empty;

    public int UserId { get; set; }

    public int ServiceId { get; set; }
}