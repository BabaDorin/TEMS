using System.Threading.Tasks;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.System_Files;

namespace temsAPI.Contracts
{
    public interface IEquipmentLabelManager
    {
        Task SetLabel(Equipment equipment);
        void SetLabel(Equipment equipment, EquipmentLabel label);
    }
}