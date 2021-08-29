using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Helpers.ReusableSnippets;
using temsAPI.System_Files;

namespace temsAPI.Helpers.EquipmentHelpers
{
    public class EquipmentLabelHelper : IEquipmentLabelHelper
    {
        /// <summary>
        /// Given an instance of equipment (With Definition and Type assigned!)
        /// Returns the appropriate EquipmentLabel
        /// </summary>
        /// <param name="equipment"></param>
        /// <returns></returns>
        public EquipmentLabel GetAppropriateLabel(Equipment equipment)
        {
            // Eligible for EquipmentLabel.Equipment
            if (EligibleForLabel_Equipment(equipment))
                return EquipmentLabel.Equipment;

            if (EligibleForLabel_Part(equipment))
                return EquipmentLabel.Part;

            // It can't be anything else
            return EquipmentLabel.Component;
        }

        public bool EligibleForLabel_Equipment(Equipment equipment)
        {
            // No parent + it's type also does not have parents
            return equipment.ParentID == null && equipment.EquipmentDefinition.EquipmentType.Parents.IsNullOrEmpty();
        }

        public bool EligibleForLabel_Part(Equipment equipment)
        {
            // Type = child type + does not have a parent
            return equipment.ParentID == null && !equipment.EquipmentDefinition.EquipmentType.Parents.IsNullOrEmpty();
        }

        public bool EligibleForLabel_Component(Equipment equipment)
        {
            // Type = child type + has a parent
            return equipment.ParentID != null;
        }
    }
}
