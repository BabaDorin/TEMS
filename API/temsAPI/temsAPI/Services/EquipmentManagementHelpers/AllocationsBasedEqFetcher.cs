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
            Expression<Func<EquipmentAllocation, bool>> whereExp = GetWhereExp(filter);

            // INCLUDE
            Func<IQueryable<EquipmentAllocation>, IIncludableQueryable<EquipmentAllocation, object>> finalIncludeExp =
                q => q.Include(q => q.Equipment)
                    .ThenInclude(q => q.EquipmentDefinition)
                    .ThenInclude(q => q.EquipmentType)
                    .Include(q => q.Room)
                    .Include(q => q.Personnel);

            return await _unitOfWork.EquipmentAllocations
                .FindAll(
                    skip: filter.Skip,
                    take: filter.Take,
                    include: finalIncludeExp,
                    where: whereExp,
                    select: q => q.Equipment
                );
        }

        public async Task<int> GetAmount(EquipmentFilter filter)
        {
            Expression<Func<EquipmentAllocation, bool>> whereExp = GetWhereExp(filter);

            // INCLUDE
            Func<IQueryable<EquipmentAllocation>, IIncludableQueryable<EquipmentAllocation, object>> finalIncludeExp =
                q => q.Include(q => q.Equipment)
                    .ThenInclude(q => q.EquipmentDefinition)
                    .ThenInclude(q => q.EquipmentType);

            return await _unitOfWork.EquipmentAllocations.Count(
                where: whereExp,
                include: finalIncludeExp);
        }

        private Expression<Func<EquipmentAllocation, bool>> GetWhereExp(EquipmentFilter filter)
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

            // Include In Use / Unused
            Expression<Func<EquipmentAllocation, bool>> eqUsingStateExp = null;
            if (!(filter.IncludeInUse && filter.IncludeUnused))
            {
                eqUsingStateExp = q => q.Equipment.IsUsed == filter.IncludeInUse;
            }

            // Include Functional / Defect
            Expression<Func<EquipmentAllocation, bool>> eqFunctionalityStateExp = null;
            if (!(filter.IncludeFunctional && filter.IncludeDefect))
            {
                eqFunctionalityStateExp = q => q.Equipment.IsDefect == !filter.IncludeFunctional;
            }

            // Include Parent / children
            Expression<Func<EquipmentAllocation, bool>> eqParentalBasedInclusionExp = null;
            if (!(filter.IncludeParents && filter.IncludeChildren))
            {
                eqParentalBasedInclusionExp = q => q.Equipment.EquipmentDefinition.ParentID == null;
            }

            // Include Attached / Detached
            Expression<Func<EquipmentAllocation, bool>> eqAttachmentExp = null;
            if (!(filter.IncludeAttached && filter.IncludeDetached))
            {
                eqAttachmentExp = q => (q.Equipment.ParentID != null) == filter.IncludeAttached;
            }

            var finalWhereExp = ExpressionCombiner.And(
            ignoreArchived,
            eqOfRoomsExp,
            eqOfPersonnelExp,
            eqOfTypeExp,
            eqOfDefinitionsExp,
            eqUsingStateExp,
            eqFunctionalityStateExp,
            eqParentalBasedInclusionExp,
            eqAttachmentExp);

            return finalWhereExp;
        }
    }
}
