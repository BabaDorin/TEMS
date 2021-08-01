using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.EquipmentEntities;

namespace temsAPI.Data.Factories.LogFactories
{
    public class ChildEquipmentDetachedParentLogFactory : ILogFactory
    {
        Equipment _parent;
        Equipment _child;
        string _createdById;

        public ChildEquipmentDetachedParentLogFactory(Equipment parent, Equipment child, string createdById)
        {
            _parent = parent;
            _child = child;
            _createdById = createdById;
        }

        public Log Create()
        {
            return new Log()
            {
                Id = Guid.NewGuid().ToString(),
                CreatedByID = _createdById,
                DateCreated = DateTime.Now,
                EquipmentID = _parent.Id,
                Description = String.Format(
                    "Child ({0}, {1}) with the TEMSID of [{2}] and Serial Number: [{3}] has been DETACHED",
                    _child.EquipmentDefinition.EquipmentType.Name,
                    _child.EquipmentDefinition.Identifier,
                    _child.TEMSID,
                    _child.SerialNumber)
            };
        }
    }
}
