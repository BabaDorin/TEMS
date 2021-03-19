using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.KeyEntities;
using temsAPI.Data.Entities.LibraryEntities;
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
            //modelBuilder.Entity<Key>()
            //    .Property(b => b.Copies)
            //    .HasDefaultValueSql("1");

            // OnDeleteCascade PropertyEquipmentTypeAssociations
            //modelBuilder.Entity<EquipmentType>()
            //    .HasMany(e => e.PropertyEquipmentTypeAssociations)
            //    .WithOne(e => e.Type)
            //    .OnDelete(DeleteBehavior.ClientCascade);

            //modelBuilder.Entity<Property>()
            //    .HasMany(e => e.PropertyEquipmentTypeAssociations)
            //    .WithOne(e => e.Property)
            //    .OnDelete(DeleteBehavior.ClientCascade);

            //OnDeleteCascade EquipmentSpecifications
            modelBuilder.Entity<EquipmentDefinition>()
                .HasMany(e => e.EquipmentSpecifications)
                .WithOne(e => e.EquipmentDefinition)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Property>()
                .HasMany(e => e.EquipmentSpecifications)
                .WithOne(e => e.Property)
                .OnDelete(DeleteBehavior.Cascade);

            // OnDeleteCascade EquipmentDefinition
            modelBuilder.Entity<EquipmentType>()
                .HasMany(e => e.EquipmentDefinitions)
                .WithOne(e => e.EquipmentType)
                .OnDelete(DeleteBehavior.Cascade);

            // OnDeleteCascade Logs
            modelBuilder.Entity<Equipment>()
                .HasMany(e => e.Logs)
                .WithOne(e => e.Equipment)
                .OnDelete(DeleteBehavior.Cascade);

            // OnDeleteCascade RoomEquipmentAllocations
            modelBuilder.Entity<Equipment>()
                .HasMany(e => e.EquipmentAllocations)
                .WithOne(e => e.Equipment)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Room>()
                .HasMany(e => e.EquipmentAllocations)
                .WithOne(e => e.Room)
                .OnDelete(DeleteBehavior.Cascade);

            // OnDeleteCascade PersonnelRoomSupervisory
            modelBuilder.Entity<Personnel>()
                .HasMany(e => e.PersonnelRoomSupervisories)
                .WithOne(e => e.Personnel)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Room>()
                .HasMany(e => e.PersonnelRoomSupervisories)
                .WithOne(e => e.Room)
                .OnDelete(DeleteBehavior.Cascade);

            // OnDeleteCascade PersonnelEquipmentAllocation
            modelBuilder.Entity<Equipment>()
                .HasMany(e => e.EquipmentAllocations)
                .WithOne(e => e.Equipment)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Personnel>()
                .HasMany(e => e.EquipmentAllocations)
                .WithOne(e => e.Personnel)
                .OnDelete(DeleteBehavior.Cascade);

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
               .OnDelete(DeleteBehavior.Cascade);

            // OnDeleteCascade KeyAllocation
            modelBuilder.Entity<Key>()
                .HasMany(e => e.KeyAllocations)
                .WithOne(e => e.Key)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Personnel>()
                .HasMany(e => e.KeyAllocations)
                .WithOne(e => e.Personnel)
                .OnDelete(DeleteBehavior.Cascade);

            // OnDeleteCascade Tickets
            //modelBuilder.Entity<Equipment>()
            //    .HasMany(e => e.Tickets)
            //    .WithOne(e => e.Equipment)
            //    .OnDelete(DeleteBehavior.Cascade);

            // OnDeleteCascade DataType
            modelBuilder.Entity<DataType>()
                .HasMany(e => e.DataTypeProperties)
                .WithOne(e => e.DataType)
                .OnDelete(DeleteBehavior.Cascade);

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
        public DbSet<PersonnelPosition> PersonnelPositions { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<EquipmentAllocation> EquipmentAllocations { get; set; }
        public DbSet<PersonnelRoomSupervisory> PersonnelRoomSupervisories { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<RoomLabel> RoomLabels { get; set; }

        // Library entities
        public DbSet<LibraryFolder> LibraryFolders { get; set; }
        public DbSet<LibraryItem> LibraryItems { get; set; }

        // Key entities
        public DbSet<Key> Keys { get; set; }
        public DbSet<KeyAllocation> KeyAllocations { get; set; }
    }
}
