using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.EquipmentEntities;

namespace temsAPI.Data.Factories.LogFactories
{
    public class EquipmentUpdatedLogFactory : ILogFactory
    {
        Equipment _equipment;
        string _createdById;

        public EquipmentUpdatedLogFactory(Equipment equipment, string createdById)
        {
            _equipment = equipment;
            _createdById = createdById;
        }

        public Log Create()
        {
            return new Log()
            {
                Id = Guid.NewGuid().ToString(),
                DateCreated = DateTime.Now,
                CreatedByID = _createdById,
                EquipmentID = _equipment.Id,
                Description = "Equipment information has been updated"
            };
        }
    }
}
