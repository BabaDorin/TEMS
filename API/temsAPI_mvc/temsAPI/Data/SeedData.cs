using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Data
{
    public static class SeedData
    {
        public static void Seed(
            UserManager<TEMSUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext dbContext
            )
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
            SeedDataTypes(dbContext);
        }

        private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            (new List<string>() { "Administrator", "Technician", "Personnel" })
                .ForEach(r =>
                {
                    if (!roleManager.RoleExistsAsync(r).Result)
                    {
                        var role = new IdentityRole { Name = r };
                        var result = roleManager.CreateAsync(role).Result;
                    }
                });
        }

        private static void SeedUsers(UserManager<TEMSUser> userManager)
        {
            if (userManager.FindByNameAsync("Administrator").Result == null)
            {
                var user = new TEMSUser { UserName = "admin@localhost", Email = "admin@localhost" };
                var result = userManager.CreateAsync(user, "Passw0rd!!").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Administrator").Wait();
                }
            }
        }

        private static void SeedDataTypes(ApplicationDbContext dbContext)
        {
            (new List<string>() { "String", "Integer", "Double", "Boolean" })
                .ForEach(r =>
                 {
                     if (!dbContext.DataTypes.Any(dt => dt.Name == r))
                     {
                         dbContext.DataTypes.Add(new DataType
                         {
                             ID = Guid.NewGuid().ToString(),
                             Name = r
                         });
                         var result = dbContext.SaveChanges();
                     }
                 });
        }
    }
}
