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
        public class PersonnelSeparator : IEquipmentSeparator
        {
            private IUnitOfWork _unitOfWork;

            public PersonnelSeparator(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public string DefaultKeyName { get; set; } = "No personnel (deposit or room)";

            private Dictionary<string, Identifiable> _visitedPersonnel = new Dictionary<string, Identifiable>();

            // Baba Dorin => Baba Dorin - Profesor, Tehnician. (O64838488)
            private IIdentifiable KeyValueDecorator(Personnel personnel)
            {
                if (_visitedPersonnel.TryGetValue(personnel.Id, out var identifiable))
                    return identifiable;

                personnel = _unitOfWork.Personnel
                    .Find<Personnel>(
                        where: q => q.Id == personnel.Id,
                        include: q => q.Include(q => q.Positions))
                    .Result
                    .FirstOrDefault();

                StringBuilder identifier = new StringBuilder();
                identifier.Append($"{personnel.Name} ");
                if (!personnel.Positions.IsNullOrEmpty())
                    identifier.Append($"- {string.Join(", ", personnel.Positions.Select(q => q.Name))}. ");

                if(!string.IsNullOrEmpty(personnel.PhoneNumber))
                    identifier.Append($"({personnel.PhoneNumber})");

                identifiable = new Identifiable(personnel.Id, identifier.ToString());
                _visitedPersonnel.Add(personnel.Id, identifiable);
                return identifiable;
            }

            public IEnumerable<IGrouping<IIdentifiable, Equipment>> GroupEquipment(IEnumerable<Equipment> equipment)
            {
                return equipment.GroupBy(q => q.EquipmentAllocations
                        .FirstOrDefault(q1 => q1.DateReturned == null && q1.PersonnelID != null) == null
                            ? null
                            : KeyValueDecorator(q.EquipmentAllocations.FirstOrDefault(q1 => q1.DateReturned == null).Personnel));
            }
        }
    }
}
