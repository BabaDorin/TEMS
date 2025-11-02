using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.KeyEntities;

namespace temsAPI.ViewModels.Key
{
    public class ViewKeyAllocationViewModel
    {
        public string Id { get; set; }
        public Option Personnel { get; set; }
        public Option Key { get; set; }
        public Option Room { get; set; }
        public Option AllocatedBy { get; set; }
        public DateTime DateAllocated { get; set; }
        public DateTime? DateReturned { get; set; }

        public static ViewKeyAllocationViewModel FromModel(KeyAllocation allocation)
        {
            return new ViewKeyAllocationViewModel
            {
                Id = allocation.Id,
                DateAllocated = allocation.DateAllocated,
                DateReturned = allocation.DateReturned,
                Personnel = new Option
                {
                    Value = allocation.PersonnelID,
                    Label = allocation.Personnel.Name,
                    Additional = string.Join(", ", allocation.Personnel.Positions.Select(q => q.Name))
                },
                Key = new Option
                {
                    Value = allocation.KeyID,
                    Label = allocation.Key.Identifier,
                },
                Room = new Option
                {
                    Value = (allocation.Key.RoomId != null)
                                ? allocation.Key.RoomId
                                : "--",
                    Label = (allocation.Key.RoomId != null)
                                ? allocation.Key.Room.Identifier
                                : "--",
                }
            };
        }
    }
}
