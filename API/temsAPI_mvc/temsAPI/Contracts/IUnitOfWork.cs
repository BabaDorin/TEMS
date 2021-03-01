using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.KeyEntities;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Contracts
{
    interface IUnitOfWork: IDisposable
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
        
        // Other entities
        IGenericRepository<Personnel> Personnel { get; }
        IGenericRepository<PersonnelEquipmentAllocation> PersonnelEquipmentAllocations { get; }
        IGenericRepository<PersonnelRoomSupervisory> PersonnelRoomSupervisories { get; }
        IGenericRepository<Room> Rooms { get; }
        IGenericRepository<RoomEquipmentAllocation> RoomEquipmentAllocations { get; }

        Task Save();
    }
}
