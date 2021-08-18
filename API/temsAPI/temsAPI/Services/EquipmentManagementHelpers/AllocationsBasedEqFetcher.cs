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

namespace temsAPI.Services.EquipmentManagementHelpers
{
    /// <summary>
    /// Fetches equipment, but from Allocations table (Useful when retrieving equipment allocated to a specific room or personnel.
    /// </summary>
    public class AllocationsBasedEqFetcher : IEquipmentFetcher
    {
        private IUnitOfWork _unitOfWork;

        public AllocationsBasedEqFetcher(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Equipment>> Fetch(EquipmentFilter filter)
        {
            // Default expression
            Expression<Func<EquipmentAllocation, bool>> ignoreArchived = q => !q.Equipment.IsArchieved;

            Expression<Func<EquipmentAllocation, bool>> eqOfRoomsExp = null;
            if (!filter.Rooms.IsNullOrEmpty())
                eqOfRoomsExp = q => q.DateReturned == null && filter.Rooms.Contains(q.RoomID);

            Expression<Func<EquipmentAllocation, bool>> eqOfPersonnelExp = null;
            if (!filter.Personnel.IsNullOrEmpty())
                eqOfPersonnelExp = q => q.DateReturned == null && filter.Personnel.Contains(q.PersonnelID);

            // Type filtering
            Expression<Func<EquipmentAllocation, bool>> eqOfTypeExp = null;
            if (!filter.Types.IsNullOrEmpty() && filter.Types.IndexOf("any") == -1)
                eqOfTypeExp = q => filter.Types.Contains(q.Equipment.EquipmentDefinition.EquipmentTypeID);

            // Definition filtering
            Expression<Func<EquipmentAllocation, bool>> eqOfDefinitionsExp = null;
            if (!filter.Definitions.IsNullOrEmpty() && filter.Definitions.IndexOf("any") == -1)
                eqOfDefinitionsExp = q => filter.Definitions.Contains(q.Equipment.EquipmentDefinitionID);

            // OnlyParents (aka Parent inclusion)
            Expression<Func<EquipmentAllocation, bool>> parentInclusionExp = null;
            if (filter.OnlyParents)
                parentInclusionExp = q => q.Equipment.EquipmentDefinition.ParentID == null;

            // OnlyDetached
            Expression<Func<EquipmentAllocation, bool>> onlyDetachedExp = null;
            if (filter.OnlyDetached)
                onlyDetachedExp = q => q.Equipment.ParentID == null;

            // Place for more where filters

            // WHERE
            var finalWhereExp = ExpressionCombiner.And(
                ignoreArchived,
                eqOfRoomsExp,
                eqOfPersonnelExp,
                eqOfTypeExp,
                eqOfDefinitionsExp,
                parentInclusionExp,
                onlyDetachedExp);

            // INCLUDE
            Func<IQueryable<EquipmentAllocation>, IIncludableQueryable<EquipmentAllocation, object>> finalIncludeExp =
                q => q.Include(q => q.Equipment).ThenInclude(q => q.EquipmentDefinition).ThenInclude(q => q.EquipmentType)
                    .Include(q => q.Room)
                    .Include(q => q.Personnel);

            return await _unitOfWork.EquipmentAllocations
                .FindAll(
                    skip: filter.Skip,
                    take: filter.Take,
                    include: finalIncludeExp,
                    where: finalWhereExp,
                    select: q => q.Equipment
                );
        }
    }
}
