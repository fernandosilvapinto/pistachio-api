namespace Pistachio.Api.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; } = false;

        // Relacionamento com Scheduling
       // public ICollection<Scheduling> Schedulings { get; set; } = new List<Scheduling>();
    }
}
