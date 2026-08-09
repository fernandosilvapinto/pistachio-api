using Pistachio.Api.Models;

namespace Pistachio.Api.DTOs.Schedulings;

public class SchedulingResponse
{
    public int Id { get; set; }

    public DateTime ScheduledDate { get; set; }

    public string ServiceName { get; set; } = string.Empty;

    public SchedulingStatus Status { get; set; }

    public int UserId { get; set; }

    public string UserName { get; set; } = string.Empty;

    public int ServiceId { get; set; }

    public string ServiceDescription { get; set; } = string.Empty;
}