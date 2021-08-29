using System.Threading.Tasks;
using temsAPI.Data.Entities.EquipmentEntities;

namespace temsAPI.Contracts
{
    public interface IEquipmentLabelManager
    {
        Task SetEquipmentLabel(Equipment equipment);
    }
}