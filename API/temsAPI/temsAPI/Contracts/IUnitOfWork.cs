using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.KeyEntities;
using temsAPI.Data.Entities.LibraryEntities;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.Report;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Contracts
{
    public interface IUnitOfWork: IDisposable
    {
        IGenericRepository<Announcement> Announcements { get; }
        IGenericRepository<FrequentTicketProblem> FrequentTicketProblems { get; }
        IGenericRepository<Log> Logs { get; }
        IGenericRepository<Ticket> Tickets { get; }
        IGenericRepository<CommonNotification> CommonNotifications { get; }
        IGenericRepository<UserNotification> UserNotifications { get; }
        IGenericRepository<Equipment> Equipments { get; }
        IGenericRepository<EquipmentDefinition> EquipmentDefinitions { get; }
        IGenericRepository<EquipmentSpecifications> EquipmentSpecifications { get; }
        IGenericRepository<EquipmentType> EquipmentTypes { get; }
        IGenericRepository<Property> Properties { get; }
        IGenericRepository<DataType> DataTypes { get; }
        IGenericRepository<PropertyEquipmentTypeAssociation> PropertyEquipmentTypeAssociations { get; }
        IGenericRepository<Key> Keys { get; }
        IGenericRepository<KeyAllocation> KeyAllocations { get; }
        IGenericRepository<TEMSUser> TEMSUsers { get; }
        IGenericRepository<Privilege> Privileges { get; }
        IGenericRepository<RolePrivileges> RolePrivileges { get; }
        IGenericRepository<LibraryItem> LibraryItems { get; }
        IGenericRepository<LibraryFolder> LibraryFolders { get; }
        IGenericRepository<ReportTemplate> ReportTemplates { get; }
        IGenericRepository<Report> Reports { get; }
        IGenericRepository<Personnel> Personnel { get; }
        IGenericRepository<EquipmentAllocation> EquipmentAllocations { get; }
        IGenericRepository<Room> Rooms { get; }
        IGenericRepository<Status> Statuses { get; }
        IGenericRepository<Label> Labels { get; }
        IGenericRepository<RoomLabel> RoomLabels { get; }
        IGenericRepository<PersonnelPosition> PersonnelPositions { get; }
        IGenericRepository<TemsJWT> JWTBlacklist { get; }
        
        Task Save();
    }
}
