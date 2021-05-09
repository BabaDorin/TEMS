using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.KeyEntities;

namespace temsAPI.ViewModels.Key
{
    public class ViewKeySimplifiedViewModel
    {
        public string Id { get; set; }
        public string Identifier { get; set; }
        public Option Room { get; set; }
        public Option AllocatedTo { get; set; }
        public string TimePassed { get; set; }
        public string Description { get; set; }

        public static ViewKeySimplifiedViewModel FromModel(Data.Entities.KeyEntities.Key key)
        {
            var lastActiveAllocation = key.KeyAllocations.FirstOrDefault(q => q.DateReturned == null);

            return new ViewKeySimplifiedViewModel()
            {
                Id = key.Id,
                Identifier = key.Identifier,
                Description = key.Description,
                Room = (key.Room != null)
                ? new Option
                {
                    Value = key.RoomId,
                    Label = key.Room.Identifier
                }
                : new Option
                {
                    Value = "--",
                    Label = "--"
                },
                AllocatedTo = (lastActiveAllocation != null)
                ? new Option
                {
                    Value = lastActiveAllocation.PersonnelID,
                    Label = lastActiveAllocation.Personnel.Name,
                }
                : new Option
                {
                    Value = "--",
                    Label = "--"
                },
                TimePassed = (lastActiveAllocation != null)
                ? $"{(DateTime.Now - lastActiveAllocation.DateAllocated).TotalMinutes:f0} minutes ago"
                : "--"
            };
        }
    }
}
