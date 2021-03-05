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

            // OnDeleteCascade PropertyEquipmentTypeAssociations
            //modelBuilder.Entity<EquipmentType>()
            //    .HasMany(e => e.PropertyEquipmentTypeAssociations)
            //    .WithOne(e => e.Type)
            //    .OnDelete(DeleteBehavior.ClientCascade);

            //modelBuilder.Entity<Property>()
            //    .HasMany(e => e.PropertyEquipmentTypeAssociations)
            //    .WithOne(e => e.Property)
            //    .OnDelete(DeleteBehavior.ClientCascade);

            // OnDeleteCascade EquipmentSpecifications
            modelBuilder.Entity<EquipmentDefinition>()
                .HasMany(e => e.EquipmentSpecifications)
                .WithOne(e => e.EquipmentDefinition)
                .OnDelete(DeleteBehavior.ClientCascade);

            // OnDeleteCascade EquipmentDefinition
            modelBuilder.Entity<EquipmentType>()
                .HasMany(e => e.EquipmentDefinitions)
                .WithOne(e => e.EquipmentType)
                .OnDelete(DeleteBehavior.ClientCascade);

            // OnDeleteCascade Logs
            modelBuilder.Entity<Equipment>()
                .HasMany(e => e.Logs)
                .WithOne(e => e.Equipment)
                .OnDelete(DeleteBehavior.ClientCascade);

            // OnDeleteCascade RoomEquipmentAllocations
            modelBuilder.Entity<Equipment>()
                .HasMany(e => e.RoomEquipmentAllocations)
                .WithOne(e => e.Equipment)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<Room>()
                .HasMany(e => e.RoomEquipmentAllocations)
                .WithOne(e => e.Room)
                .OnDelete(DeleteBehavior.ClientCascade);

            // OnDeleteCascade PersonnelRoomSupervisory
            modelBuilder.Entity<Personnel>()
                .HasMany(e => e.PersonnelRoomSupervisories)
                .WithOne(e => e.Personnel)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<Room>()
                .HasMany(e => e.PersonnelRoomSupervisories)
                .WithOne(e => e.Room)
                .OnDelete(DeleteBehavior.ClientCascade);

            // OnDeleteCascade PersonnelEquipmentAllocation
            modelBuilder.Entity<Equipment>()
                .HasMany(e => e.PersonnelEquipmentAllocations)
                .WithOne(e => e.Equipment)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<Personnel>()
                .HasMany(e => e.PersonnelEquipmentAllocations)
                .WithOne(e => e.Personnel)
                .OnDelete(DeleteBehavior.ClientCascade);

            // OnDeleteCascade Equipment
            modelBuilder.Entity<Equipment>()
                .HasMany(e => e.Children)
                .WithOne(e => e.Parent)
                .OnDelete(DeleteBehavior.ClientCascade);

            // OnDeleteCascade EquipmentDefinition
            modelBuilder.Entity<EquipmentDefinition>()
                .HasMany(e => e.Children)
                .WithOne(e => e.Parent)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<EquipmentType>()
               .HasMany(e => e.EquipmentTypeKinships)
               .WithOne(e => e.ParentEquipmentType)
               .OnDelete(DeleteBehavior.ClientCascade);

            // OnDeleteCascade KeyAllocation
            modelBuilder.Entity<Key>()
                .HasMany(e => e.KeyAllocations)
                .WithOne(e => e.Key)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<Personnel>()
                .HasMany(e => e.KeyAllocations)
                .WithOne(e => e.Personnel)
                .OnDelete(DeleteBehavior.ClientCascade);

            // OnDeleteCascade Tickets
            modelBuilder.Entity<Equipment>()
                .HasMany(e => e.Tickets)
                .WithOne(e => e.Equipment)
                .OnDelete(DeleteBehavior.ClientCascade);

            base.OnModelCreating(modelBuilder);
        }


        // Equipment entities
        public DbSet<DataType> DataTypes { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<EquipmentType> EquipmentTypes { get; set; }
        public DbSet<EquipmentTypeKinship> EquipmentTypeKinships { get; set; }
        public DbSet<PropertyEquipmentTypeAssociation> PropertyEquipmentTypeAssociations { get; set; }
        public DbSet<EquipmentSpecifications> EquipmentSpecifications { get; set; }
        public DbSet<EquipmentDefinition> EquipmentDefinitions { get; set; }
        public DbSet<Equipment> Equipments { get; set; }

        // User entities
        public DbSet<TEMSUser> TEMSUsers { get; set; }
        public DbSet<Privilege> Privileges { get; set; }
        public DbSet<RolePrivileges> RolePrivileges { get; set; }

        // Communication entitites
        public DbSet<Announcement> Announcements { get; set; }
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
