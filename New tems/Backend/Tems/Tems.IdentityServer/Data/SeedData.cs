using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;
using Tems.Common.Identity;

namespace Tems.IdentityServer.Data;

public static class SeedData
{
    public static async Task EnsureSeedDataAsync(IServiceProvider serviceProvider)
    {
        var database = serviceProvider.GetRequiredService<IMongoDatabase>();
        var users = database.GetCollection<User>("users");
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            var adminExists = await users.Find(u => u.Username == "admin").AnyAsync();
            
            if (!adminExists)
            {
                var passwordHasher = new PasswordHasher<User>();
                var admin = new User
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Username = "admin",
                    Email = "admin@tems.local",
                    FullName = "System Administrator",
                    PhoneNumber = "",
                    IsActive = true,
                    Roles = new List<string>(), // Roles managed by Keycloak
                    Claims = new Dictionary<string, string>(), // Claims managed by Keycloak
                    CreatedAt = DateTime.UtcNow
                };

                admin.PasswordHash = passwordHasher.HashPassword(admin, "Admin123!");
                await users.InsertOneAsync(admin);
                
                logger.LogInformation("Admin user created successfully with username: admin");
            }
            else
            {
                logger.LogInformation("Admin user already exists, skipping seed");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding database");
        }
    }
}
