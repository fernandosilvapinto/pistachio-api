namespace Pistachio.Api.DTOs.Payments;

public class CreatePaymentRequest
{
    public decimal Amount { get; set; }

    public string Status { get; set; } = "Pending";

    public int UserId { get; set; }

    public int? SchedulingId { get; set; }

    public int ServiceId { get; set; }
}