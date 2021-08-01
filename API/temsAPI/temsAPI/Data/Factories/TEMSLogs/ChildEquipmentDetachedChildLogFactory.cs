using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.EquipmentEntities;

namespace temsAPI.Data.Factories.LogFactories
{
    public class ChildEquipmentDetachedChildLogFactory : ILogFactory
    {
        Equipment _parent;
        Equipment _child;
        string _createdById;

        public ChildEquipmentDetachedChildLogFactory(Equipment parent, Equipment child, string createdById)
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
                EquipmentID = _child.Id,
                Description = String.Format(
                    "Has been DETACHED from parent ({0}, {1}) with the TEMSID of [{2}] and Serial Number: [{3}]",
                    _parent.EquipmentDefinition.EquipmentType.Name,
                    _parent.EquipmentDefinition.Identifier,
                    _parent.TEMSID,
                    _parent.SerialNumber)
            };
        }
    }
}
