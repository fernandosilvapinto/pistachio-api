using Pistachio.Api.Models;

namespace Pistachio.Api.DTOs.Schedulings;

public class UpdateSchedulingStatusRequest
{
    public SchedulingStatus Status { get; set; }
}
