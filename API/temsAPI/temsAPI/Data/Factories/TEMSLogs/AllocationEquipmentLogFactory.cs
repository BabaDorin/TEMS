using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Services;

namespace temsAPI.Data.Factories.LogFactories
{
    public class AllocationEquipmentLogFactory : ILogFactory
    {
        private EquipmentAllocation _allocation;
        private string _createdById;

        public AllocationEquipmentLogFactory(EquipmentAllocation equipmentAllocation, string createdById = null)
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
                DateCreated = DateTime.Now,
                EquipmentID = _allocation.EquipmentID,
                Description = String.Format("Has been allocated to {0}.",
                _allocation.Personnel != null
                    ? _allocation.Personnel.Name
                    : _allocation.Room.Identifier)
            };
        }
    }
}
