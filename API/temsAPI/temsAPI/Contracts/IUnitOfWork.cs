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
        // Communication entities
        IGenericRepository<Announcement> Announcements { get; }
        IGenericRepository<FrequentTicketProblem> FrequentTicketProblems { get; }
        IGenericRepository<Log> Logs { get; }
        IGenericRepository<LogType> LogTypes { get; }
        IGenericRepository<Ticket> Tickets { get; }
        IGenericRepository<ToDo> ToDos { get; }

        // Equipment entities
        IGenericRepository<Equipment> Equipments { get; }
        IGenericRepository<EquipmentDefinition> EquipmentDefinitions { get; }
        IGenericRepository<EquipmentSpecifications> EquipmentSpecifications { get; }
        IGenericRepository<EquipmentType> EquipmentTypes { get; }
        IGenericRepository<Property> Properties { get; }
        IGenericRepository<DataType> DataTypes { get; }
        IGenericRepository<PropertyEquipmentTypeAssociation> PropertyEquipmentTypeAssociations { get; }

        // Key entities
        IGenericRepository<Key> Keys { get; }
        IGenericRepository<KeyAllocation> KeyAllocations { get; }
        
        // User entities
        IGenericRepository<TEMSUser> TEMSUsers { get; }
        IGenericRepository<Privilege> Privileges { get; }
        IGenericRepository<RolePrivileges> RolePrivileges { get; }


        // Library entities
        IGenericRepository<LibraryItem> LibraryItems { get; }
        IGenericRepository<LibraryFolder> LibraryFolders { get; }

        // Report templates
        IGenericRepository<ReportTemplate> ReportTemplates { get; }
        IGenericRepository<Report> Reports { get; }

        // Other entities
        IGenericRepository<Personnel> Personnel { get; }
        IGenericRepository<EquipmentAllocation> EquipmentAllocations { get; }
        IGenericRepository<PersonnelRoomSupervisory> PersonnelRoomSupervisories { get; }
        IGenericRepository<Room> Rooms { get; }
        IGenericRepository<Status> Statuses { get; }
        IGenericRepository<Label> Labels { get; }
        IGenericRepository<RoomLabel> RoomLabels { get; }
        IGenericRepository<PersonnelPosition> PersonnelPositions { get; }
        IGenericRepository<TemsJWT> JWTBlacklist { get; }

        Task Save();
    }
}
