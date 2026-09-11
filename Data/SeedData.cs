using Pistachio.Api.Models;

namespace Pistachio.Api.Data;

/// <summary>
/// Dados de domínio para desenvolvimento. Não cria identidades: as pessoas
/// autenticam-se no Keeper, e as linhas de User aqui são apenas referências
/// locais, sem credenciais, reclamadas no primeiro login por email verificado.
/// </summary>
public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        if (context.Users.Any())
            return;

        var people = new List<User>
        {
            new() { Name = "Admin User", Email = "admin@pistachio.local" },
            new() { Name = "Workshop Manager", Email = "manager@pistachio.local" },
            new() { Name = "Senior Mechanic", Email = "mechanic@pistachio.local" },
            new() { Name = "Junior Mechanic", Email = "mechanic2@pistachio.local" },
            new() { Name = "John Rider", Email = "customer@pistachio.local" },
            new() { Name = "Maria Santos", Email = "maria@pistachio.local" },
            new() { Name = "Carlos Ferreira", Email = "carlos@pistachio.local" }
        };

        context.Users.AddRange(people);

        await context.SaveChangesAsync();

        var seniorMechanic = people[2];
        var juniorMechanic = people[3];
        var john = people[4];
        var maria = people[5];
        var carlos = people[6];

        var services = new List<Service>
        {
            new()
            {
                Name = "Oil Change",
                Description = "Engine oil and filter replacement",
                Price = 49.99m,
                IsActive = true,
                IsFeatured = true
            },

            new()
            {
                Name = "Brake Inspection",
                Description = "Complete brake system inspection",
                Price = 39.99m,
                IsActive = true
            },

            new()
            {
                Name = "Tyre Replacement",
                Description = "Motorcycle tyre replacement",
                Price = 149.99m,
                IsActive = true,
                IsFeatured = true
            },

            new()
            {
                Name = "General Service",
                Description = "Scheduled maintenance service",
                Price = 199.99m,
                IsActive = true
            },

            new()
            {
                Name = "Electrical Diagnosis",
                Description = "Electronic fault diagnosis",
                Price = 89.99m,
                IsActive = true
            },

            new() // inativo, para testar filtragem
            {
                Name = "Carburetor Tuning (Legacy)",
                Description = "Serviço descontinuado, mantido só para histórico",
                Price = 59.99m,
                IsActive = false
            }
        };

        context.Services.AddRange(services);

        await context.SaveChangesAsync();

        var schedulings = new List<Scheduling>
        {
            new()
            {
                ScheduledDate = DateTime.UtcNow.AddDays(3),
                ServiceName = services[0].Name,
                Status = SchedulingStatus.Pending,
                UserId = john.Id,
                ServiceId = services[0].Id
            },

            new()
            {
                ScheduledDate = DateTime.UtcNow.AddDays(5),
                ServiceName = services[1].Name,
                Status = SchedulingStatus.Confirmed,
                UserId = john.Id,
                ServiceId = services[1].Id,
                AssigneeId = seniorMechanic.Id
            },

            new()
            {
                ScheduledDate = DateTime.UtcNow.AddDays(-2),
                ServiceName = services[2].Name,
                Status = SchedulingStatus.Completed,
                UserId = maria.Id,
                ServiceId = services[2].Id,
                AssigneeId = juniorMechanic.Id
            },

            new()
            {
                ScheduledDate = DateTime.UtcNow.AddDays(7),
                ServiceName = services[3].Name,
                Status = SchedulingStatus.Pending,
                UserId = maria.Id,
                ServiceId = services[3].Id
            },

            new()
            {
                ScheduledDate = DateTime.UtcNow.AddDays(1),
                ServiceName = services[4].Name,
                Status = SchedulingStatus.Cancelled,
                UserId = carlos.Id,
                ServiceId = services[4].Id
            },

            new()
            {
                ScheduledDate = DateTime.UtcNow.AddDays(-5),
                ServiceName = services[0].Name,
                Status = SchedulingStatus.Completed,
                UserId = carlos.Id,
                ServiceId = services[0].Id,
                AssigneeId = seniorMechanic.Id
            }
        };

        context.Schedulings.AddRange(schedulings);

        await context.SaveChangesAsync();

        var payments = new List<Payment>
        {
            new()
            {
                Amount = services[2].Price,
                Status = "Paid",
                PaymentDate = DateTime.UtcNow.AddDays(-2),
                UserId = maria.Id,
                ServiceId = services[2].Id,
                SchedulingId = schedulings[2].Id
            },

            new()
            {
                Amount = services[0].Price,
                Status = "Paid",
                PaymentDate = DateTime.UtcNow.AddDays(-5),
                UserId = carlos.Id,
                ServiceId = services[0].Id,
                SchedulingId = schedulings[5].Id
            },

            new()
            {
                Amount = services[1].Price,
                Status = "Unpaid",
                PaymentDate = DateTime.UtcNow,
                UserId = john.Id,
                ServiceId = services[1].Id,
                SchedulingId = schedulings[1].Id
            }
        };

        context.Payments.AddRange(payments);

        await context.SaveChangesAsync();
    }
}
