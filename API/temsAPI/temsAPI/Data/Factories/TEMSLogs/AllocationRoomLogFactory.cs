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

        public AllocationRoomLogFactory(EquipmentAllocation equipmentAllocation)
        {
            _allocation = equipmentAllocation;
        }

        public Log Create()
        {
            return new Log()
            {
                Id = Guid.NewGuid().ToString(),
                RoomID = _allocation.RoomID,
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
