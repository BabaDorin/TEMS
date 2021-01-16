using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.KeyEntities;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Key>()
                .Property(b => b.Copies)
                .HasDefaultValueSql("1");

            base.OnModelCreating(modelBuilder);
        }

        // Equipment entities
        public DbSet<DataType> DataTypes { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<EquipmentType> EquipmentTypes { get; set; }
        public DbSet<PropertyEquipmentTypeAssociation> PropertyEquipmentTypeAssociations { get; set; }
        public DbSet<EquipmentSpecifications> EquipmentSpecifications { get; set; }
        public DbSet<EquipmentDefinition> EquipmentDefinitions { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<EquipmentDefinitionKinship> EquipmentDefinitionKinships { get; set; }

        // User entities
        public DbSet<Privilege> Privileges { get; set; }
        public DbSet<RolePrivileges> RolePrivileges { get; set; }

        // Communication entitites
        public DbSet<Announcement> Announcements{ get; set; }
        public DbSet<FrequentTicketProblem> FrequentTicketProblems { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<ToDo> ToDos { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<LogType> LogTypes { get; set; }

        // Other entities
        public DbSet<Personnel> Personnel { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<PersonnelEquipmentAllocation> PersonnelEquipmentAllocations { get; set; }
        public DbSet<PersonnelRoomSupervisory> PersonnelRoomSupervisories { get; set; }
        public DbSet<RoomEquipmentAllocation> RoomEquipmentAllocations { get; set; }


        // Key entities
        public DbSet<Key> Keys { get; set; }
        public DbSet<KeyAllocation> KeyAllocations { get; set; }
    }
}
