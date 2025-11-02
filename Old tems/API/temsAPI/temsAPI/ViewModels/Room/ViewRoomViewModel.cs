using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.ViewModels.Personnel;

namespace temsAPI.ViewModels.Room
{
    public class ViewRoomViewModel
    {
        public string Id { get; set; }
        public string Identifier { get; set; }
        public int Floor { get; set; }
        public string Description { get; set; }
        public int ActiveTickets { get; set; }
        public List<Option> Supervisories { get; set; } = new List<Option>();
        public List<Option> Labels { get; set; } = new List<Option>();
        public bool IsArchieved { get; set; }

        public static ViewRoomViewModel FromModel(Data.Entities.OtherEntities.Room room)
        {
            return new ViewRoomViewModel
            {
                Id = room.Id,
                Description = room.Description,
                Identifier = room.Identifier,
                Floor = room.Floor ?? 0,
                IsArchieved = room.IsArchieved,
                ActiveTickets = room.Tickets.Count(q => q.DateClosed == null),
                Labels = room.Labels.Select(q => new Option
                {
                    Value = q.Id,
                    Label = q.Name
                }).ToList(),
                Supervisories = room.Supervisories.Select(q => new Option
                {
                    Value = q.Id,
                    Label = q.Name
                }).ToList(),
            };
        }
    }
}
