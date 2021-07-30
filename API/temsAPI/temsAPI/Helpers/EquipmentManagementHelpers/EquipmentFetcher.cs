using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Helpers.Filters;
using temsAPI.Helpers.ReusableSnippets;

namespace temsAPI.Helpers.EquipmentManagementHelpers
{
    /// <summary>
    /// Fetches equipment using the unit of work, based on information provided by the filter
    /// </summary>
    public class EquipmentFetcher
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

            IEnumerable<Equipment> equipment;

            if(!filter.Rooms.IsNullOrEmpty() || !filter.Personnel.IsNullOrEmpty())
            {
                equipment = await FetchFromAllocations(filter); 
            }
            else
            {
                equipment = await FetchFromEquipment(filter);
            }

            return equipment;
        }

        private async Task<IEnumerable<Equipment>> FetchFromEquipment(EquipmentFilter filter)
        {
            Expression<Func<Equipment, bool>> eqOfTypeExp = null;
            if (!filter.Types.IsNullOrEmpty() && filter.Types.IndexOf("any") == -1)
                eqOfTypeExp = q => filter.Types.Contains(q.EquipmentDefinition.EquipmentTypeID);

            // Place for more where filters
            
            // WHERE
            var finalWhereExp = eqOfTypeExp;

            // INCLUDE
            Func<IQueryable<Equipment>, IIncludableQueryable<Equipment, object>> finalIncludeExp =
                q => q.Include(q => q.EquipmentDefinition).ThenInclude(q => q.EquipmentType)
                    .Include(q => q.EquipmentAllocations.Where(q => q.DateReturned == null))
                    .ThenInclude(q => q.Room)
                    .Include(q => q.EquipmentAllocations.Where(q => q.DateReturned == null))
                    .ThenInclude(q => q.Personnel);

            return await _unitOfWork.Equipments
                .FindAll<Equipment>(
                    skip: 0,
                    take: 1111,
                    where: finalWhereExp,
                    include: finalIncludeExp
                );
        }

        private async Task<IEnumerable<Equipment>> FetchFromAllocations(EquipmentFilter filter)
        {
            Expression<Func<EquipmentAllocation, bool>> eqOfRoomsExp = null;
            if (!filter.Rooms.IsNullOrEmpty())
                eqOfRoomsExp = q => q.DateReturned == null && filter.Rooms.Contains(q.RoomID);

            Expression<Func<EquipmentAllocation, bool>> eqOfPersonnelExp = null;
            if (!filter.Personnel.IsNullOrEmpty())
                eqOfPersonnelExp = q => q.DateReturned == null && filter.Personnel.Contains(q.PersonnelID);

            Expression<Func<EquipmentAllocation, bool>> eqOfTypeExp = null;
            if (!filter.Types.IsNullOrEmpty() && filter.Types.IndexOf("any") == -1)
                eqOfTypeExp = q => filter.Types.Contains(q.Equipment.EquipmentDefinition.EquipmentTypeID);

            // Place for more where filters

            // WHERE
            var finalWhereExp = ExpressionCombiner.And(eqOfRoomsExp, eqOfPersonnelExp, eqOfTypeExp);

            // INCLUDE
            Func<IQueryable<EquipmentAllocation>, IIncludableQueryable<EquipmentAllocation, object>> finalIncludeExp =
                q => q.Include(q => q.Equipment).ThenInclude(q => q.EquipmentDefinition).ThenInclude(q => q.EquipmentType)
                    .Include(q => q.Room)
                    .Include(q => q.Personnel);

            return await _unitOfWork.EquipmentAllocations
                .FindAll(
                    skip: filter.GetSkip(),
                    take: filter.GetTake(),
                    where: finalWhereExp,
                    include: finalIncludeExp,
                    select: q => q.Equipment
                );
        }
    }
}
