using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.OtherEntities;

namespace temsAPI.Data.Factories.LogFactories
{
    public class RoomSupervisoriesChangedLogFactory : ILogFactory
    {
        Room _room;
        string _createdById;

        public RoomSupervisoriesChangedLogFactory(Room room, string createdById)
        {
            _room = room;
            _createdById = createdById;
        }

        public Log Create()
        {
            return new Log()
            {
                Id = Guid.NewGuid().ToString(),
                DateCreated = DateTime.Now,
                CreatedByID = _createdById,
                RoomID = _room.Id,
                Description = "Room's supervisories information has been updated"
            };
        }
    }
}
