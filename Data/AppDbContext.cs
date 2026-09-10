using Microsoft.EntityFrameworkCore;
using Pistachio.Api.Models;

namespace Pistachio.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Scheduling> Schedulings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Service> Services { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Scheduling.User (dono do agendamento) — apagar o User não deve apagar o Scheduling em cascata
            modelBuilder.Entity<Scheduling>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Scheduling.Assignee — opcional; se a pessoa for apagada, o campo fica null
            modelBuilder.Entity<Scheduling>()
                .HasOne(s => s.Assignee)
                .WithMany()
                .HasForeignKey(s => s.AssigneeId)
                .OnDelete(DeleteBehavior.SetNull);

            // Subject é o claim "sub" do Keeper. Vazio significa conta por reclamar,
            // criada pelo domínio antes de a pessoa se ter autenticado alguma vez.
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Subject)
                .IsUnique()
                .HasFilter("\"Subject\" <> ''");
        }
    }
}
