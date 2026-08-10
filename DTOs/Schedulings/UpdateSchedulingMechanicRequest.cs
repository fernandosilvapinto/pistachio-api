namespace Pistachio.Api.DTOs.Schedulings;

public class UpdateSchedulingMechanicRequest
{
    // Null remove a atribuição (agendamento fica sem mecânico)
    public int? AssignedMechanicId { get; set; }
}
