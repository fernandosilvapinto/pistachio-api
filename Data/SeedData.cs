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

            new()
            {
                Name = "Senior Mechanic",
                Email = "mechanic@pistachio.local",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pistachio123!"),
                RoleId = mechanicRole.Id
            },

            new()
            {
                Name = "John Rider",
                Email = "customer@pistachio.local",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pistachio123!"),
                RoleId = customerRole.Id
            }
        };

        context.Users.AddRange(users);

        await context.SaveChangesAsync();

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
            }
        };

        context.Services.AddRange(services);

        await context.SaveChangesAsync();

        var scheduling = new Scheduling
        {
            ScheduledDate = DateTime.UtcNow.AddDays(3),
            ServiceName = services[0].Name,
            UserId = users[3].Id,
            ServiceId = services[0].Id
        };

        context.Schedulings.Add(scheduling);

        await context.SaveChangesAsync();

        var payment = new Payment
        {
            Amount = services[0].Price,
            Status = "Paid",
            PaymentDate = DateTime.UtcNow,
            UserId = users[3].Id,
            ServiceId = services[0].Id,
            SchedulingId = scheduling.Id
        };

        context.Payments.Add(payment);

        await context.SaveChangesAsync();
    }
}