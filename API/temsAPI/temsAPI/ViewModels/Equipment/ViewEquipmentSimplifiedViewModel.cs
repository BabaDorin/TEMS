using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.Equipment
{
    public class ViewEquipmentSimplifiedViewModel
    {
        public string Id { get; set; }
        public string TemsId { get; set; }
        public string SerialNumber { get; set; }
        public string TemsIdOrSerialNumber { get; set; }
        public string Definition { get; set; }
        public string Assignee { get; set; }
        public string Type { get; set; }
        public bool IsUsed { get; set; }
        public bool IsDefect { get; set; }
        public bool IsArchieved { get; set; }

        public static ViewEquipmentSimplifiedViewModel FromEquipment(Data.Entities.EquipmentEntities.Equipment eq)
        {
            ViewEquipmentSimplifiedViewModel viewEquipmentSimplified = new ViewEquipmentSimplifiedViewModel
            {
                Id = eq.Id,
                IsDefect = eq.IsDefect,
                IsUsed = eq.IsUsed,
                IsArchieved = eq.IsArchieved,
                TemsId = eq.TEMSID,
                SerialNumber = eq.SerialNumber,
                Type = eq.EquipmentDefinition.EquipmentType.Name,
                Definition = eq.EquipmentDefinition.Identifier,
            };

            var lastAllocation = eq.EquipmentAllocations
                .FirstOrDefault(q => !q.IsArchieved && q.DateReturned == null);

            if (lastAllocation == null)
                viewEquipmentSimplified.Assignee = "Deposit";
            else
                viewEquipmentSimplified.Assignee =
                    (lastAllocation.Room != null)
                    ? "Room: " + lastAllocation.Room.Identifier
                    : "Personnel: " + lastAllocation.Personnel.Name;

            viewEquipmentSimplified.TemsIdOrSerialNumber =
                String.IsNullOrEmpty(viewEquipmentSimplified.TemsId)
                ? viewEquipmentSimplified.SerialNumber
                : viewEquipmentSimplified.TemsId;

            return viewEquipmentSimplified;
        }
    }
}
