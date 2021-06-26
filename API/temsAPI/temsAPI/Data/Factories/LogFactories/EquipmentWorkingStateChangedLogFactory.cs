using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.EquipmentEntities;

namespace temsAPI.Data.Factories.LogFactories
{
    public class EquipmentWorkingStateChangedLogFactory : ILogFactory
    {
        Equipment _equipment;
        string _createdById;

        public EquipmentWorkingStateChangedLogFactory(Equipment equipment, string createdById)
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
                Description = _equipment.IsDefect
                    ? "Has been marked as DEFECT"
                    : "Has been marked as WORKING"
            };
        }
    }
}
