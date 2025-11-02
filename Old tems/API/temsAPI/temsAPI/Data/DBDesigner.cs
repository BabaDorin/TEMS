using Microsoft.EntityFrameworkCore;
using System;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.KeyEntities;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.System_Files.Internationalization;

namespace temsAPI.Data
{
    public class DBDesigner : IDBDesigner
    {
        /// <summary>
        /// Configures db models, which include setting on-delete behaviours, default values, column constraints and other
        /// </summary>
        /// <param name="modelbuilder"></param>
        public void ConfigureModels(ModelBuilder modelbuilder)
        {
            SetOnDeleteBehaviour(modelbuilder);
            SetDefaultValues(modelbuilder);
            SetCustomRelations(modelbuilder);
            SetColumnConstraints(modelbuilder);
        }

        private void SetColumnConstraints(ModelBuilder modelBuilder)
        {
            // Identity on TrackingNumber
            modelBuilder.Entity<Ticket>(e =>
            {
                e.Property(e => e.TrackingNumber).ValueGeneratedOnAdd();
            });
        }

        private void SetDefaultValues(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Property>()
                .Property(e => e.EditablePropertyInfo)
                .HasDefaultValue(true);

            modelBuilder.Entity<EquipmentType>()
                .Property(e => e.EditableTypeInfo)
                .HasDefaultValue(true);

            modelBuilder.Entity<TEMSUser>()
                .Property(e => e.PrefferedLang)
                .HasDefaultValue(Enum.GetName(Lang.en));
        }

        private void SetOnDeleteBehaviour(ModelBuilder modelBuilder)
        {
            // On delete cascade: Property => EquipmentTypeSpecifications:
            modelBuilder.Entity<Property>()
                .HasMany(e => e.EquipmentSpecifications)
                .WithOne(e => e.Property)
                .OnDelete(DeleteBehavior.Cascade);

            // OnDeleteCascade: EquipmentDefinition => EquipmentSpecifications
            modelBuilder.Entity<EquipmentDefinition>()
                .HasMany(e => e.EquipmentSpecifications)
                .WithOne(e => e.EquipmentDefinition)
                .OnDelete(DeleteBehavior.Cascade);

            // OnDeleteCascade: Equipment => EquipmentAllocations
            modelBuilder.Entity<Equipment>()
                .HasMany(e => e.EquipmentAllocations)
                .WithOne(e => e.Equipment)
                .OnDelete(DeleteBehavior.Cascade);

            // OnDeleteCascade: Equipmet => Logs
            modelBuilder.Entity<Equipment>()
                .HasMany(e => e.Logs)
                .WithOne(e => e.Equipment)
                .OnDelete(DeleteBehavior.Cascade);

            // OnDeleteCascade: Personnel => Logs
            modelBuilder.Entity<Personnel>()
                .HasMany(e => e.Logs)
                .WithOne(e => e.Personnel)
                .OnDelete(DeleteBehavior.Cascade);

            // OnDeleteCascade: Personnel => EquipmentAllocations
            modelBuilder.Entity<Personnel>()
                .HasMany(e => e.EquipmentAllocations)
                .WithOne(e => e.Personnel)
                .OnDelete(DeleteBehavior.Cascade);

            // OnDeleteCascade: Personnel => KeyAllocations
            modelBuilder.Entity<Personnel>()
                .HasMany(e => e.KeyAllocations)
                .WithOne(e => e.Personnel)
                .OnDelete(DeleteBehavior.Cascade);

            // OnDeleteCascade: Room => Logs
            modelBuilder.Entity<Room>()
                .HasMany(e => e.Logs)
                .WithOne(e => e.Room)
                .OnDelete(DeleteBehavior.Cascade);

            // OnDeleteCascade: Room => EquipmentAllocations
            modelBuilder.Entity<Room>()
                .HasMany(e => e.EquipmentAllocations)
                .WithOne(e => e.Room)
                .OnDelete(DeleteBehavior.Cascade);

            // OnDeleteCascade: Room => keys
            modelBuilder.Entity<Room>()
                .HasMany(e => e.Keys)
                .WithOne(e => e.Room)
                .OnDelete(DeleteBehavior.Cascade);

            // OnDeleteCascade: Equipment => Allocations
            modelBuilder.Entity<Equipment>()
                .HasMany(e => e.EquipmentAllocations)
                .WithOne(e => e.Equipment)
                .OnDelete(DeleteBehavior.Cascade);

            // OnDeleteCascade: Keys => KeyAllocations
            modelBuilder.Entity<Key>()
                .HasMany(e => e.KeyAllocations)
                .WithOne(e => e.Key)
                .OnDelete(DeleteBehavior.Cascade);

            // OnDeleteCascade DataType => Properties
            modelBuilder.Entity<DataType>()
                .HasMany(e => e.DataTypeProperties)
                .WithOne(e => e.DataType)
                .OnDelete(DeleteBehavior.Cascade);

            // OnDeleteCascade User => UserNotifications
            modelBuilder.Entity<TEMSUser>()
                .HasMany(e => e.UserNotifications)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);

            // OnDeleteCascade User => UserWithBlacklistedToken
            modelBuilder.Entity<TEMSUser>()
                .HasOne(e => e.TokenBlacklisting)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void SetCustomRelations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserCommonNotification>()
                .HasKey(c => new { c.UserId, c.NotificationId });
        }
    }
}
