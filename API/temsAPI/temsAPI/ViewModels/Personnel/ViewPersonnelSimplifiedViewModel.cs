using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.Personnel
{
    public class ViewPersonnelSimplifiedViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int AllocatedEquipments { get; set; }
        public int ActiveTickets { get; set; }
        public string Positions { get; set; }
        public bool IsArchieved { get; set; }

        public static ViewPersonnelSimplifiedViewModel FromModel(
            Data.Entities.OtherEntities.Personnel personnel)
        {
            return new ViewPersonnelSimplifiedViewModel
            {
                Id = personnel.Id,
                Name = personnel.Name,
                ActiveTickets = personnel.Tickets.Count(q => q.DateClosed == null),
                AllocatedEquipments = personnel.EquipmentAllocations.Count(q => q.DateReturned == null),
                Positions = (personnel.Positions != null)
                            ? string.Join(", ", personnel.Positions.Select(q => q.Name))
                            : "",
                IsArchieved = personnel.IsArchieved
            };
        }
    }
}
