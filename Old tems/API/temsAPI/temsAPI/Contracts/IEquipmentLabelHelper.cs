using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.System_Files;

namespace temsAPI.Contracts
{
    public interface IEquipmentLabelHelper
    {
        EquipmentLabel GetAppropriateLabel(Equipment equipment);
        bool EligibleForLabel_Equipment(Equipment equipment);
        bool EligibleForLabel_Part(Equipment equipment);
        bool EligibleForLabel_Component(Equipment equipment);
    }
}
