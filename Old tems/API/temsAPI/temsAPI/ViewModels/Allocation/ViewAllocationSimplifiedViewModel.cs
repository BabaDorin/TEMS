using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.ViewModels.Equipment;

namespace temsAPI.ViewModels.Allocation
{
    public class ViewAllocationSimplifiedViewModel
    {
        public string Id { get; set; }
        public Option Equipment { get; set; }
        public Option Personnel { get; set; }
        public Option Room { get; set; }
        public DateTime DateAllocated { get; set; }
        public DateTime? DateReturned { get; set; }

        public static ViewAllocationSimplifiedViewModel FromModel(EquipmentAllocation allocation)
        {
            return new ViewAllocationSimplifiedViewModel
            {
                Id = allocation.Id,
                DateAllocated = allocation.DateAllocated,
                DateReturned = allocation.DateReturned,
                Equipment = new Option
                {
                    Value = allocation.Equipment.Id,
                    Label = allocation.Equipment.TemsIdOrSerialNumber,
                    Additional = allocation.Equipment.EquipmentDefinition.Identifier
                },
                Personnel = (allocation.Personnel == null)
                ? null
                : new Option
                {
                    Value = allocation.Personnel.Id,
                    Label = allocation.Personnel.Name,
                },
                Room = (allocation.Room == null)
                ? null
                : new Option
                {
                    Value = allocation.Room.Id,
                    Label = allocation.Room.Identifier,
                },
            };
        }
    }
}
