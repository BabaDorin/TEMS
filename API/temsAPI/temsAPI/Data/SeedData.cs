using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.OtherEntities;
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
            SeedTickedStatuses(dbContext);
            SeedRoomLabels(dbContext);
            SeedPersonnelPositions(dbContext);
            SeedPrivileges(dbContext, roleManager, userManager);
        }

        private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            var roles = new List<string>() { "Administrator", "Technician", "User", };
                
            foreach(var r in roles)
            {
                if (roleManager.Roles == null || !roleManager.RoleExistsAsync(r).Result) //null
                {
                    var role = new IdentityRole { Name = r };
                    var result = roleManager.CreateAsync(role).Result;
                }
            }
        }

        private static void SeedUsers(UserManager<TEMSUser> userManager)
        {
            if (userManager.FindByNameAsync("tems@dmin").Result == null)
            {
                var user = new TEMSUser { UserName = "tems@dmin", Email = "tems@dmin" };
                var result = userManager.CreateAsync(user, "ef2e0d52ca46923e48697b12d565a222").Result; // https://ro.wikisource.org/wiki/Luceafărul_(Eminescu) I x2
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Administrator").Wait();
                }
            }
        }

        private static void SeedDataTypes(ApplicationDbContext dbContext)
        {
            (new List<string>() { "text", "number", "bool" })
                .ForEach(r =>
                 {
                     if (!dbContext.DataTypes.Any(dt => dt.Name == r))
                     {
                         dbContext.DataTypes.Add(new DataType
                         {
                             Id = Guid.NewGuid().ToString(),
                             Name = r
                         });
                     }
                 });

            dbContext.SaveChanges();
        }

        private static void SeedTickedStatuses(ApplicationDbContext dbContext)
        {
            int importanceIndex = 0;
            (new List<string>() { "Urgent", "Medium", "Future" })
                .ForEach(r =>
                {
                    if (!dbContext.Statuses.Any(lt => lt.Name== r))
                    {
                        dbContext.Statuses.Add(new Status
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = r,
                            ImportanceIndex = importanceIndex++
                        });
                    }
                });

            dbContext.SaveChanges();
        }

        private static void SeedRoomLabels(ApplicationDbContext dbContext)
        {
            (new List<string>() { "Deposit", "Library", "Office", "Meeting Room", "Class Room", "Laboratory" })
                .ForEach(r =>
                {
                    if (!dbContext.RoomLabels.Any(lt => lt.Name == r))
                    {
                        dbContext.RoomLabels.Add(new RoomLabel
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = r,
                        });
                    }
                });

            dbContext.SaveChanges();
        }

        private static void SeedPersonnelPositions(ApplicationDbContext dbContext)
        {
            (new List<string>() { "Professor", "Auxiliary Worker", "Technician", "Management"})
                .ForEach(r =>
                {
                    if (!dbContext.PersonnelPositions.Any(lt => lt.Name == r))
                    {
                        dbContext.PersonnelPositions.Add(new PersonnelPosition
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = r,
                        });
                    }
                });

            dbContext.SaveChanges();
        }

        private static void SeedPrivileges(
            ApplicationDbContext dbContext, 
            RoleManager<IdentityRole> roleManager,
            UserManager<TEMSUser> userManager)
        {
            List<Privilege> privileges = new List<Privilege>
            {
                new Privilege
                {
                    Id = Guid.NewGuid().ToString(),
                    Identifier = "Can manage Entities",
                    Description = "Can create, update or remove equipments, rooms, personnel and much more."
                },
                new Privilege
                {
                    Id = Guid.NewGuid().ToString(),
                    Identifier = "Can view Entities",
                    Description = "Can view equipments, rooms, personnel and much more."
                },
                new Privilege
                {
                    Id = Guid.NewGuid().ToString(),
                    Identifier = "Can manage announcements",
                    Description = "Can create, update or delete global announcements"
                },
                new Privilege
                {
                    Id = Guid.NewGuid().ToString(),
                    Identifier = "Can manage system configuration",
                    Description = "Can create, update or delete types, definitions, properties etc."
                },
                new Privilege
                {
                    Id = Guid.NewGuid().ToString(),
                    Identifier = "Can send emails",
                    Description = "Can send email to all of registered personnel"
                },
                new Privilege
                {
                    Id = Guid.NewGuid().ToString(),
                    Identifier = "Can allocate keys",
                    Description = "Can allocate keys to personnel"
                },
            };

           
            
            privileges.ForEach(q =>
            {
                if (!dbContext.Privileges.Any(p => p.Identifier == q.Identifier))
                {
                    dbContext.Privileges.Add(q);
                }
            });
            
            dbContext.SaveChanges();

            // Seed admin claims
            var admin = roleManager.FindByNameAsync("Administrator").Result;
            List<string> adminClaims = roleManager.GetClaimsAsync(admin).Result
                .Select(q => q.Type).ToList();

            List<string> adminClaimsToAdd = privileges.Select(q => q.Identifier)
                .Except(adminClaims)
                .ToList();

            IdentityResult result = null;
            foreach (var item in adminClaimsToAdd)
                result = roleManager.AddClaimAsync(admin, new Claim(item, "ye")).Result;

            // Seed user claims
            var user = roleManager.FindByNameAsync("User").Result;
            List<string> currentUserClaims = roleManager.GetClaimsAsync(user).Result
                .Select(q => q.Type).ToList();
            List<string> userClaims = new()
            {
                "Can view Entities"
            };

            foreach (var item in userClaims.Except(currentUserClaims))
                result = roleManager.AddClaimAsync(user, new Claim(item, "ye")).Result;

            // Seed technician claims
            var technician = roleManager.FindByNameAsync("Technician").Result;
            List<string> currentTechnicianClaims = roleManager.GetClaimsAsync(technician).Result
                .Select(q => q.Type).ToList();
            List<string> technicianClaims = new()
            {
                "Can view Entities",
                "Can manage Entities"
            };
            foreach (var item in technicianClaims.Except(currentTechnicianClaims))
                result = roleManager.AddClaimAsync(technician, new Claim(item, "ye")).Result;
        }
    }
}
