namespace Pistachio.Api.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Admin, Client, etc.

        // Relacionamento com User (um papel pode ter vários usuários)
        //public ICollection<User> Users { get; set; } = new List<User>();
    }
}
