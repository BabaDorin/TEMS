using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            SeedProperties(dbContext);
            SeedLogTypes(dbContext);
            SeedTickedStatuses(dbContext);
            SeedRoomLabels(dbContext);
            SeedPersonnelPositions(dbContext);
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

        private static void SeedProperties(ApplicationDbContext dbContext)
        {
            //var seedProperties = new List<string>() { "Model", "Manufacturer" };

            //seedProperties.ForEach(prop =>
            //{
            //    // (Display name) Billing Address => (name) billingAddress
            //    string propName = Regex.Replace(
            //        prop[0].ToString().ToLower() + prop.Substring(1, prop.Length-1).Trim(),
            //        @"\s+", "");

            //    if (!dbContext.Properties.Any(qu => qu.Name == propName))
            //    {
            //        dbContext.Properties.Add(new Property
            //        {
            //            Id = Guid.NewGuid().ToString(),
            //            DataType = dbContext.DataTypes.ToList()[0],
            //            DisplayName = prop,
            //            Name = propName,
            //            DataTypeID = dbContext.DataTypes.ToList()[0].Id,
            //        });
            //    }
            //});

            //dbContext.SaveChanges();
        }

        private static void SeedLogTypes(ApplicationDbContext dbContext)
        {
            (new List<string>() { "Repair", "Maintenance", "Allocation" })
                .ForEach(r =>
                {
                    if (!dbContext.LogTypes.Any(lt => lt.Type == r))
                    {
                        dbContext.LogTypes.Add(new LogType
                        {
                            Id = Guid.NewGuid().ToString(),
                            Type = r
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
    }
}
