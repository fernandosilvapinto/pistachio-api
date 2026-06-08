namespace Pistachio.Api.DTOs.Payments;

public class PaymentResponse
{
    public int Id { get; set; }

    public decimal Amount { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime PaymentDate { get; set; }

    public int UserId { get; set; }

    public int? SchedulingId { get; set; }

    public int ServiceId { get; set; }
}