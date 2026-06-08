namespace Pistachio.Api.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Paid, Cancelled
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        // Relacionamento com User
        public int UserId { get; set; }
        public User? User { get; set; }

        // Relacionamento com Scheduling (opcional)
        public int? SchedulingId { get; set; }
        public Scheduling? Scheduling { get; set; }
        
        // Relacionamento com Service
        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;

    }
}
