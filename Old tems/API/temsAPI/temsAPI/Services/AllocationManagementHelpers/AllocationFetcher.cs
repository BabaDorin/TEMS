using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Helpers;
using temsAPI.Helpers.Filters;
using temsAPI.Helpers.ReusableSnippets;

namespace temsAPI.Services.AllocationManagementHelpers
{
    public class AllocationFetcher : IFetcher<EquipmentAllocation, AllocationFilter>
    {
        readonly IUnitOfWork _unitOfWork;

        public AllocationFetcher(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<EquipmentAllocation>> Fetch(AllocationFilter filter)
        {
            // Where
            var finalWhere = GetWhereExp(filter);

            // Include
            Func<IQueryable<EquipmentAllocation>, IIncludableQueryable<EquipmentAllocation, object>> includeExp
                = q => q.Include(q => q.Room)
                        .Include(q => q.Personnel)
                        .Include(q => q.Equipment)
                        .ThenInclude(q => q.EquipmentDefinition);

            // Order by
            Func<IQueryable<EquipmentAllocation>, IOrderedQueryable<EquipmentAllocation>> orderByExp;

            // If only closed allocations are included, order by return date. Otherwise - by allocation date.
            if (filter.IncludeStatuses.IsNullOrEmpty() || filter.IncludeStatuses.Count() == 1 || filter.IncludeStatuses.IndexOf("Closed") > -1)
                orderByExp = q => q.OrderByDescending(q => q.DateReturned);
            else
                orderByExp = q => q.OrderByDescending(q => q.DateAllocated);

            return await _unitOfWork.EquipmentAllocations
                .FindAll<EquipmentAllocation>(
                    include: includeExp,
                    where: finalWhere);
        }

        public async Task<int> GetAmount(AllocationFilter filter)
        {
            var whereExp = GetWhereExp(filter);
            return await _unitOfWork.EquipmentAllocations
                .Count(whereExp);
        }

        private Expression<Func<EquipmentAllocation, bool>> GetWhereExp(AllocationFilter filter)
        {
            // Default
            Expression<Func<EquipmentAllocation, bool>> defaultWhereExp = q => !q.IsArchieved;

            // Of Equipment
            Expression<Func<EquipmentAllocation, bool>> alOfEquipmentExp = null;
            if (!filter.Equipment.IsNullOrEmpty())
                alOfEquipmentExp = q => filter.Equipment.Contains(q.EquipmentID);

            // Of Room
            Expression<Func<EquipmentAllocation, bool>> alOfRoom = null;
            if (!filter.Rooms.IsNullOrEmpty())
                alOfRoom = q => q.RoomID != null && filter.Rooms.Contains(q.RoomID);

            // Of Personnel
            Expression<Func<EquipmentAllocation, bool>> alOfPersonnel = null;
            if (!filter.Personnel.IsNullOrEmpty())
                alOfPersonnel = q => q.PersonnelID != null && filter.Personnel.Contains(q.PersonnelID);

            // Of Definitions
            Expression<Func<EquipmentAllocation, bool>> alOfDefinition = null;
            if (!filter.Definitions.IsNullOrEmpty())
                alOfDefinition = q => filter.Definitions.Contains(q.Equipment.EquipmentDefinitionID);
            

            // Include labels
            Expression<Func<EquipmentAllocation, bool>> alOfLabels = null;
            if(filter.Equipment.IsNullOrEmpty() && !filter.IncludeLabels.IsNullOrEmpty())
                alOfLabels = q => filter.IncludeLabels.Contains(q.EquipmentLabel);

            // Include statuses 
            Expression<Func<EquipmentAllocation, bool>> alOfStatuses = null;
            if (!filter.IncludeStatuses.IsNullOrEmpty())
            {
                var activeStatusIndex = filter.IncludeStatuses.IndexOf("Active");
                var closedStatusIndex = filter.IncludeStatuses.IndexOf("Closed");

                // If both true or both false => return all of the allocations
                if (!((activeStatusIndex == -1 && closedStatusIndex == -1) || (activeStatusIndex > -1 && closedStatusIndex > -1)))
                    alOfStatuses = q => (q.DateReturned == null) == activeStatusIndex > -1;
            }

            Expression<Func<EquipmentAllocation, bool>> finalWhere = ExpressionCombiner.And(
                defaultWhereExp,
                alOfEquipmentExp,
                alOfRoom,
                alOfPersonnel,
                alOfDefinition,
                alOfLabels,
                alOfStatuses);

            return finalWhere;
        }
    }
}
