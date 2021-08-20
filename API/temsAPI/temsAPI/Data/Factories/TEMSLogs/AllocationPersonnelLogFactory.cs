using System;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.EquipmentEntities;

namespace temsAPI.Data.Factories.LogFactories
{
    public class AllocationPersonnelLogFactory : ILogFactory
    {
        private EquipmentAllocation _allocation;
        private string _createdById;

        public AllocationPersonnelLogFactory(EquipmentAllocation equipmentAllocation, string createdById = null)
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
                PersonnelID = _allocation.PersonnelID,
                DateCreated = DateTime.Now,
                Description = String.Format(
                    "Was assigned the equipment ({0}, {1}) with the TEMSID of: {2} and Serial Number: {3}.",
                    _allocation.Equipment.EquipmentDefinition.EquipmentType.Name,
                    _allocation.Equipment.EquipmentDefinition.Identifier,
                    _allocation.Equipment.TEMSID,
                    _allocation.Equipment.SerialNumber
                )
            };
        }
    }
}
