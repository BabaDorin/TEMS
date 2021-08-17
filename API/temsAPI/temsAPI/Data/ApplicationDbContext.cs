using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
            new DBDesigner().ConfigureModels(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<DataType> DataTypes { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<EquipmentType> EquipmentTypes { get; set; }
        public DbSet<PropertyEquipmentTypeAssociation> PropertyEquipmentTypeAssociations { get; set; }
        public DbSet<EquipmentSpecifications> EquipmentSpecifications { get; set; }
        public DbSet<EquipmentDefinition> EquipmentDefinitions { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<TEMSUser> TEMSUsers { get; set; }
        public DbSet<Privilege> Privileges { get; set; }
        public DbSet<RolePrivileges> RolePrivileges { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<FrequentTicketProblem> FrequentTicketProblems { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<CommonNotification> CommonNotifications { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<Personnel> Personnel { get; set; }
        public DbSet<PersonnelPosition> PersonnelPositions { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<EquipmentAllocation> EquipmentAllocations { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<RoomLabel> RoomLabels { get; set; }
        public DbSet<TemsJWT> JWTBlacklist { get; set; }
        public DbSet<LibraryFolder> LibraryFolders { get; set; }
        public DbSet<LibraryItem> LibraryItems { get; set; }
        public DbSet<ReportTemplate> ReportTemplates { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Key> Keys { get; set; }
        public DbSet<KeyAllocation> KeyAllocations { get; set; }
        public DbSet<BugReport> BugReports { get; set; }
    }
}
