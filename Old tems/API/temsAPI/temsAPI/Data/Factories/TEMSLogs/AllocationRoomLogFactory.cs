using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.EquipmentEntities;

namespace temsAPI.Data.Factories.LogFactories
{
    public class AllocationRoomLogFactory : ILogFactory
    {
        private EquipmentAllocation _allocation;
        private string _createdById;

        public AllocationRoomLogFactory(EquipmentAllocation equipmentAllocation, string createdById = null)
        {
            _allocation = equipmentAllocation;
            _createdById = createdById;
        }

        public Log Create()
        {
            return new Log()
            {
                Id = Guid.NewGuid().ToString(),
                CreatedByID = _createdById,
                RoomID = _allocation.RoomID,
                EquipmentLabel = _allocation.Equipment?.Label,
                DateCreated = DateTime.Now,
                Description = String.Format(
                    "The equipment ({0}, {1}) with the TEMSID of: {2} and Serial Number: {3} has been moved here.",
                    _allocation.Equipment.EquipmentDefinition.EquipmentType.Name,
                    _allocation.Equipment.EquipmentDefinition.Identifier,
                    _allocation.Equipment.TEMSID,
                    _allocation.Equipment.SerialNumber
                )
            };
        }
    }
}
