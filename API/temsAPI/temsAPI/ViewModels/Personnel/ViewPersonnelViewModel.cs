using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.Personnel
{
    public class ViewPersonnelViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public List<Option> Positions { get; set; } = new List<Option>();
        public List<Option> RoomSupervisories { get; set; }
        public int ActiveTickets { get; set; }
        public int AllocatedEquipments { get; set; }
        public bool IsArchieved { get; set; }

        public static ViewPersonnelViewModel FromModel(Data.Entities.OtherEntities.Personnel personnel)
        {
            return new ViewPersonnelViewModel
            {
                Id = personnel.Id,
                Name = personnel.Name,
                Email = personnel.Email,
                PhoneNumber = personnel.PhoneNumber,
                IsArchieved = personnel.IsArchieved,
                ActiveTickets = personnel.Tickets.Count(q => q.DateClosed == null),
                AllocatedEquipments = personnel.EquipmentAllocations.Count(q => q.DateReturned == null),
                Positions = personnel.Positions.Select(q => new Option
                {
                    Value = q.Id,
                    Label = q.Name
                }).ToList(),
                RoomSupervisories = personnel.RoomsSupervisoried.Select(q => new Option
                {
                    Value = q.Id,
                    Label = q.Identifier
                }).ToList(),
            };
        }
    }
}
