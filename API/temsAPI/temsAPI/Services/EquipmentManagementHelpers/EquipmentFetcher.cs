using System.Collections.Generic;
using System.Linq;
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
        // EquipmentFetcher delegates it's tasks to one of the following specialized equipment fetchers:
        // AllocationsBasedEqFetcher => Fetches equipment based on Allocations table, useful when working with allocatees
        // EquipmentBasedEqFetcher => Fetches equipment based on Equipment table (When there is no allocatee specified)

        readonly IUnitOfWork _unitOfWork;

        private AllocationsBasedEqFetcher allocationsBasedEquipmentFetcher;
        private AllocationsBasedEqFetcher AllocationsBasedEquipmentFetcher
        {
            get 
            {
                if(allocationsBasedEquipmentFetcher == null)
                    allocationsBasedEquipmentFetcher = new AllocationsBasedEqFetcher(_unitOfWork);

                return allocationsBasedEquipmentFetcher; 
            }
        }

        private EquipmentBasedEqFetcher equipmentBasedEquipmentFetcher;
        private EquipmentBasedEqFetcher EquipmentBasedEquipmentFetcher
        {
            get
            {
                if (equipmentBasedEquipmentFetcher == null)
                    equipmentBasedEquipmentFetcher = new EquipmentBasedEqFetcher(_unitOfWork);

                return equipmentBasedEquipmentFetcher;
            }
        }

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

            IEquipmentFetcher equipmentFetcher = SelectEqFetcher(filter);
            return await equipmentFetcher.Fetch(filter);
        }

        /// <summary>
        /// Get the amount of equipment that satisfy the specified filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<int> GetAmount(EquipmentFilter filter)
        {
            IEquipmentFetcher equipmentFetcher = SelectEqFetcher(filter);
            return await equipmentFetcher.GetAmount(filter);
        }

        private IEquipmentFetcher SelectEqFetcher(EquipmentFilter filter)
        {
            return filter.IsAnyAllocateeSpecified()
                ? AllocationsBasedEquipmentFetcher
                : EquipmentBasedEquipmentFetcher;
        }
    }
}
