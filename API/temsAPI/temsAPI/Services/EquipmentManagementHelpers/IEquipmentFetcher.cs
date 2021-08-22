using System.Collections.Generic;
using System.Threading.Tasks;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Helpers.Filters;

namespace temsAPI.Services.EquipmentManagementHelpers
{
    public interface IEquipmentFetcher
    {
        Task<IEnumerable<Equipment>> Fetch(EquipmentFilter filter);
        Task<int> GetAmount(EquipmentFilter filter);
    }
}