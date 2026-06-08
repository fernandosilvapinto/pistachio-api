namespace Pistachio.Api.Models
{
    public class Scheduling
    {
        public int Id { get; set; }
        public DateTime ScheduledDate { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        
        // Relacionamento com User
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Relacionamento com Service
        public int ServiceId { get; set; }
        public Service? Service { get; set; }
    }
}
