namespace Pistachio.Api.Models
{
    public class Scheduling
    {
        public int Id { get; set; }
        public DateTime ScheduledDate { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public SchedulingStatus Status { get; set; } = SchedulingStatus.Pending;

        // Relacionamento com User
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Relacionamento com Service
        public int ServiceId { get; set; }
        public Service? Service { get; set; }

        // Quem vai executar o serviço (opcional — pode ainda não estar atribuído)
        public int? AssigneeId { get; set; }
        public User? Assignee { get; set; }
    }
}
