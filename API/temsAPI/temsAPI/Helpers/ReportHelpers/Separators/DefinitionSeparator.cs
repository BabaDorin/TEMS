using System.Collections.Generic;
using System.Linq;
using System.Text;
using temsAPI.Contracts;
using temsAPI.Data.Entities;
using temsAPI.Data.Entities.EquipmentEntities;

namespace temsAPI.Helpers
{

    public partial class ReportHelper
    {
        public class DefinitionSeparator : IEquipmentSeparator
        {
            public string DefaultKeyName { get; set; } = "Other";

            private Dictionary<string, Identifiable> _visitedDefinitions = new Dictionary<string, Identifiable>();

            // NVidia GeForce GTX 1060 => NVidia GeForce GTX 1060 (GPU)
            private IIdentifiable KeyValueDecorator(EquipmentDefinition definition)
            {
                if (_visitedDefinitions.TryGetValue(definition.Id, out var identifiable))
                    return identifiable;

                StringBuilder identifier = new StringBuilder();
                identifier.Append(definition.Identifier);
                if (definition.EquipmentType != null)
                    identifier.Append($" ({definition.EquipmentType.Name})");

                identifiable = new Identifiable(definition.Id, identifier.ToString());
                _visitedDefinitions.Add(definition.Id, identifiable);
                return identifiable;
            }

            public IEnumerable<IGrouping<IIdentifiable, Equipment>> GroupEquipment(IEnumerable<Equipment> equipment)
            {
                return equipment.GroupBy(q => KeyValueDecorator(q.EquipmentDefinition)).ToList();
            }
        }
    }
}
