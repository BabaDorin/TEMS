using FluentEmail.Core;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using temsAPI.Contracts;
using temsAPI.Data.Entities;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Helpers.ReusableSnippets;

namespace temsAPI.Helpers
{

    public partial class ReportHelper
    {
        public class RoomSeparator : IEquipmentSeparator
        {
            private IUnitOfWork _unitOfWork;

            public RoomSeparator(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public string DefaultKeyName { get; set; } = "No room (deposit or personnel)";

            private Dictionary<string, Identifiable> _visitedRooms = new Dictionary<string, Identifiable>();

            // 214 => Room: 214 (Laboratory, Class room). Supervisors: B.Dorin, P. Gheorghe
            private IIdentifiable KeyValueDecorator(Room room)
            {
                if (_visitedRooms.TryGetValue(room.Id, out var identifiable))
                    return identifiable;

                room = _unitOfWork.Rooms
                    .Find<Room>(
                        where: q => q.Id == room.Id,
                        include: q => q.Include(q => q.Labels)
                                       .Include(q => q.Supervisories))
                    .Result
                    .FirstOrDefault();

                StringBuilder identifier = new StringBuilder();
                identifier.Append($"Room: {room.Identifier} ");
                if (!room.Labels.IsNullOrEmpty())
                    identifier.Append($"({string.Join(", ", room.Labels.Select(q => q.Name))}). ");

                if (!room.Supervisories.IsNullOrEmpty())
                    identifier.Append($"Supervisors: {string.Join(", ", room.Supervisories.Select(q => q.ShortName))}");

                identifiable = new Identifiable(room.Id, identifier.ToString());
                _visitedRooms.Add(room.Id, identifiable);
                return identifiable;
            }

            public IEnumerable<IGrouping<IIdentifiable, Equipment>> GroupEquipment(IEnumerable<Equipment> equipment)
            {
                return equipment.GroupBy(q => q.EquipmentAllocations.FirstOrDefault(q1 => q1.DateReturned == null && q1.RoomID != null) == null
                            ? null
                            : KeyValueDecorator(q.EquipmentAllocations.FirstOrDefault(q1 => q1.DateReturned == null).Room));
            }
        }
    }
}
