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

        public AllocationEquipmentLogFactory(EquipmentAllocation equipmentAllocation)
        {
            _allocation = equipmentAllocation;
        }

        public Log Create()
        {
            return new Log()
            {
                Id = Guid.NewGuid().ToString(),
                DateCreated = DateTime.Now,
                EquipmentID = _allocation.EquipmentID,
            };
        }
    }
}
