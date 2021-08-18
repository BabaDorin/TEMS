using System.Collections.Generic;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Helpers.Filters;

namespace temsAPI.Services.EquipmentManagementHelpers
{
    /// <summary>
    /// Fetches equipment using the unit of work, based on information provided by the filter
    /// </summary>
    public class EquipmentFetcher : IEquipmentFetcher
    {
        private IUnitOfWork _unitOfWork;

        public EquipmentFetcher(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Fetched equipment based on the provided filter
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Equipment>> Fetch(EquipmentFilter filter)
        {
            // Fetch equipment from EquipmentAllocations if there is any allocatee specified
            // Otherwise - from Equipment table

            IEquipmentFetcher equipmentFetcher = filter.IsAnyAllocateeSpecified() 
                ? new AllocationsBasedEqFetcher(_unitOfWork)
                : new EquipmentBasedEqFetcher(_unitOfWork);

            return await equipmentFetcher.Fetch(filter);
        }
    }
}
