using Pistachio.Api.Models;

namespace Pistachio.Api.Data;

//Pistachio123!

public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        if (context.Roles.Any())
            return;

        var adminRole = new Role { Name = "Admin" };
        var managerRole = new Role { Name = "Manager" };
        var mechanicRole = new Role { Name = "Mechanic" };
        var customerRole = new Role { Name = "Customer" };

        context.Roles.AddRange(
            adminRole,
            managerRole,
            mechanicRole,
            customerRole
        );

        await context.SaveChangesAsync();

        var users = new List<User>
        {
            new()
            {
                Name = "Admin User",
                Email = "admin@pistachio.local",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pistachio123!"),
                RoleId = adminRole.Id
            },

            new()
            {
                Name = "Workshop Manager",
                Email = "manager@pistachio.local",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pistachio123!"),
                RoleId = managerRole.Id
            },

            new() // users[2] — mecânico
            {
                Name = "Senior Mechanic",
                Email = "mechanic@pistachio.local",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pistachio123!"),
                RoleId = mechanicRole.Id
            },

            new() // users[3] — mecânico
            {
                Name = "Junior Mechanic",
                Email = "mechanic2@pistachio.local",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pistachio123!"),
                RoleId = mechanicRole.Id
            },

            new() // users[4] — cliente
            {
                Name = "John Rider",
                Email = "customer@pistachio.local",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pistachio123!"),
                RoleId = customerRole.Id
            },

            new() // users[5] — cliente
            {
                Name = "Maria Santos",
                Email = "maria@pistachio.local",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pistachio123!"),
                RoleId = customerRole.Id
            },

            new() // users[6] — cliente
            {
                Name = "Carlos Ferreira",
                Email = "carlos@pistachio.local",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pistachio123!"),
                RoleId = customerRole.Id
            }
        };

        context.Users.AddRange(users);

        await context.SaveChangesAsync();

        var seniorMechanic = users[2];
        var juniorMechanic = users[3];
        var john = users[4];
        var maria = users[5];
        var carlos = users[6];

        var services = new List<Service>
        {
            new() // services[0]
            {
                Name = "Oil Change",
                Description = "Engine oil and filter replacement",
                Price = 49.99m,
                IsActive = true,
                IsFeatured = true
            },

            new() // services[1]
            {
                Name = "Brake Inspection",
                Description = "Complete brake system inspection",
                Price = 39.99m,
                IsActive = true
            },

            new() // services[2]
            {
                Name = "Tyre Replacement",
                Description = "Motorcycle tyre replacement",
                Price = 149.99m,
                IsActive = true,
                IsFeatured = true
            },

            new() // services[3]
            {
                Name = "General Service",
                Description = "Scheduled maintenance service",
                Price = 199.99m,
                IsActive = true
            },

            new() // services[4]
            {
                Name = "Electrical Diagnosis",
                Description = "Electronic fault diagnosis",
                Price = 89.99m,
                IsActive = true
            },

            new() // services[5] — inativo, para testar filtragem
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
            new() // schedulings[0]
            {
                ScheduledDate = DateTime.UtcNow.AddDays(3),
                ServiceName = services[0].Name,
                Status = SchedulingStatus.Pending,
                UserId = john.Id,
                ServiceId = services[0].Id
            },

            new() // schedulings[1]
            {
                ScheduledDate = DateTime.UtcNow.AddDays(5),
                ServiceName = services[1].Name,
                Status = SchedulingStatus.Confirmed,
                UserId = john.Id,
                ServiceId = services[1].Id,
                AssignedMechanicId = seniorMechanic.Id
            },

            new() // schedulings[2]
            {
                ScheduledDate = DateTime.UtcNow.AddDays(-2),
                ServiceName = services[2].Name,
                Status = SchedulingStatus.Completed,
                UserId = maria.Id,
                ServiceId = services[2].Id,
                AssignedMechanicId = juniorMechanic.Id
            },

            new() // schedulings[3]
            {
                ScheduledDate = DateTime.UtcNow.AddDays(7),
                ServiceName = services[3].Name,
                Status = SchedulingStatus.Pending,
                UserId = maria.Id,
                ServiceId = services[3].Id
            },

            new() // schedulings[4]
            {
                ScheduledDate = DateTime.UtcNow.AddDays(1),
                ServiceName = services[4].Name,
                Status = SchedulingStatus.Cancelled,
                UserId = carlos.Id,
                ServiceId = services[4].Id
            },

            new() // schedulings[5]
            {
                ScheduledDate = DateTime.UtcNow.AddDays(-5),
                ServiceName = services[0].Name,
                Status = SchedulingStatus.Completed,
                UserId = carlos.Id,
                ServiceId = services[0].Id,
                AssignedMechanicId = seniorMechanic.Id
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
