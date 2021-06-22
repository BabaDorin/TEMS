﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.KeyEntities;
using temsAPI.Data.Entities.LibraryEntities;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.Report;
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
            // On delete cascade: Property => EquipmentTypeSpecifications:
            modelBuilder.Entity<Property>()
                .HasMany(e => e.EquipmentSpecifications)
                .WithOne(e => e.Property)
                .OnDelete(DeleteBehavior.Cascade);

            // OnDeleteCascade: EquipmentType => EquipmentDefinitions
            modelBuilder.Entity<EquipmentType>()
                .HasMany(e => e.EquipmentDefinitions)
                .WithOne(e => e.EquipmentType)
                .OnDelete(DeleteBehavior.Cascade);

            // OnDeleteCascade: EquipmentDefinition => Equipment
            modelBuilder.Entity<EquipmentDefinition>()
                .HasMany(e => e.Equipment)
                .WithOne(e => e.EquipmentDefinition)
                .OnDelete(DeleteBehavior.Cascade);

            // OnDeleteCascade: EquipmentDefinition => EquipmentSpecifications
            modelBuilder.Entity<EquipmentDefinition>()
                .HasMany(e => e.EquipmentSpecifications)
                .WithOne(e => e.EquipmentDefinition)
                .OnDelete(DeleteBehavior.Cascade);

            // OnDeleteCascade: EquipmentDefinition => Child EquipmentDefinition
            modelBuilder.Entity<EquipmentDefinition>()
                .HasMany(e => e.Children)
                .WithOne(e => e.Parent)
                .OnDelete(DeleteBehavior.ClientSetNull);

            //  OnDeleteCascade: Equipment => Child Equipment
            modelBuilder.Entity<Equipment>()
                .HasMany(e => e.Children)
                .WithOne(e => e.Parent)
                .OnDelete(DeleteBehavior.ClientCascade);

            // OnDeleteCascade: Equipmet => Logs
            modelBuilder.Entity<Equipment>()
                .HasMany(e => e.Logs)
                .WithOne(e => e.Equipment)
                .OnDelete(DeleteBehavior.Cascade);

            // OnDeleteCascade: Equipment => Allocations
            modelBuilder.Entity<Equipment>()
                .HasMany(e => e.EquipmentAllocations)
                .WithOne(e => e.Equipment)
                .OnDelete(DeleteBehavior.Cascade);

            // OnDeleteCascade DataType
            modelBuilder.Entity<DataType>()
                .HasMany(e => e.DataTypeProperties)
                .WithOne(e => e.DataType)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TEMSUser>()
                .HasMany(e => e.Announcements)
                .WithOne(e => e.Author)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<TEMSUser>()
                .HasMany(e => e.ArchievedEquipment)
                .WithOne(e => e.ArchievedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TEMSUser>()
                .HasMany(e => e.RegisteredEquipment)
                .WithOne(e => e.RegisteredBy)
                .OnDelete(DeleteBehavior.ClientNoAction);

            modelBuilder.Entity<TEMSUser>()
                .HasMany(e => e.UserNotifications)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TEMSUser>()
                .HasMany(e => e.ArchievedTickets)
                .WithOne(e => e.ArchievedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TEMSUser>()
                .HasMany(e => e.CreatedTickets)
                .WithOne(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TEMSUser>()
                .HasMany(e => e.ClosedTickets)
                .WithOne(e => e.ClosedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TEMSUser>()
                .HasMany(e => e.GeneratedReports)
                .WithOne(e => e.GeneratedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TEMSUser>()
                .HasMany(e => e.CreatedLogs)
                .WithOne(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TEMSUser>()
                .HasMany(e => e.ArchivedLogs)
                .WithOne(e => e.ArchievedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TEMSUser>()
                .HasMany(e => e.ArchivedAllocations)
                .WithOne(e => e.ArchievedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TEMSUser>()
                .HasMany(e => e.ArchivedDefinitions)
                .WithOne(e => e.ArchievedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TEMSUser>()
                .HasMany(e => e.ArchivedSpecifications)
                .WithOne(e => e.ArchievedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TEMSUser>()
                .HasMany(e => e.ArchivedTypes)
                .WithOne(e => e.ArchievedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TEMSUser>()
                .HasMany(e => e.ArchivedProperties)
                .WithOne(e => e.ArchievedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TEMSUser>()
                .HasMany(e => e.ArchivedKeys)
                .WithOne(e => e.ArchievedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TEMSUser>()
                .HasMany(e => e.ArchivedKeyAllocations)
                .WithOne(e => e.ArchievedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TEMSUser>()
                .HasMany(e => e.UploadedLibraryItems)
                .WithOne(e => e.UploadedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TEMSUser>()
                .HasMany(e => e.ArchivedPersonnel)
                .WithOne(e => e.ArchievedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TEMSUser>()
                .HasMany(e => e.ArchivedPersonnelPositions)
                .WithOne(e => e.ArchievedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TEMSUser>()
                .HasMany(e => e.ArchivedRooms)
                .WithOne(e => e.ArchievedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TEMSUser>()
                .HasMany(e => e.ArchivedRoomLabels)
                .WithOne(e => e.ArchievedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TEMSUser>()
                .HasMany(e => e.CreatedReportTemplates)
                .WithOne(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TEMSUser>()
                .HasMany(e => e.ArchivedReportTemplates)
                .WithOne(e => e.ArchievedBy)
                .OnDelete(DeleteBehavior.NoAction);

            // Default values:
            modelBuilder.Entity<Property>()
                .Property(e => e.EditablePropertyInfo)
                .HasDefaultValue(true);

            modelBuilder.Entity<EquipmentType>()
                .Property(e => e.EditableTypeInfo)
                .HasDefaultValue(true);
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<UserCommonNotification>()
                .HasKey(c => new { c.UserId, c.NotificationId });

            modelBuilder.Entity<Ticket>(e =>
            {
                e.Property(e => e.TrackingNumber).ValueGeneratedOnAdd();
            });
        }

        // Equipment entities
        public DbSet<DataType> DataTypes { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<EquipmentType> EquipmentTypes { get; set; }
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
        public DbSet<CommonNotification> CommonNotifications { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }

        // Other entities
        public DbSet<Personnel> Personnel { get; set; }
        public DbSet<PersonnelPosition> PersonnelPositions { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<EquipmentAllocation> EquipmentAllocations { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<RoomLabel> RoomLabels { get; set; }
        public DbSet<TemsJWT> JWTBlacklist { get; set; }

        // Library entities
        public DbSet<LibraryFolder> LibraryFolders { get; set; }
        public DbSet<LibraryItem> LibraryItems { get; set; }

        // Report entities
        public DbSet<ReportTemplate> ReportTemplates { get; set; }
        public DbSet<Report> Reports { get; set; }

        // Key entities
        public DbSet<Key> Keys { get; set; }
        public DbSet<KeyAllocation> KeyAllocations { get; set; }
    }
}
