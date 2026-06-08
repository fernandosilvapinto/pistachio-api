namespace Pistachio.Api.DTOs.Payments;

public class UpdatePaymentRequest
{
    public decimal Amount { get; set; }

    public string Status { get; set; } = string.Empty;

    public int UserId { get; set; }

    public int? SchedulingId { get; set; }

    public int ServiceId { get; set; }
}