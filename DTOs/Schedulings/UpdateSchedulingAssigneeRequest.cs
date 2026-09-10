namespace Pistachio.Api.DTOs.Schedulings;

public class UpdateSchedulingAssigneeRequest
{
    // Null remove a atribuição (o agendamento fica sem responsável)
    public int? AssigneeId { get; set; }
}
