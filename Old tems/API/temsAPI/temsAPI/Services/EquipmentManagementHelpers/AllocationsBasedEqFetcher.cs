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
using temsAPI.System_Files;

namespace temsAPI.Services.EquipmentManagementHelpers
{
    /// <summary>
    /// Fetches equipment, but from Allocations table (Useful when retrieving equipment allocated to a specific room or personnel.
    /// </summary>
    public class AllocationsBasedEqFetcher : IFetcher<Equipment, EquipmentFilter>
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
            // BEFREE : Add default include statement here + merge with include provided by the user
            Func<IQueryable<EquipmentAllocation>, IIncludableQueryable<EquipmentAllocation, object>> finalIncludeExp =
                q => q.Include(q => q.Equipment)
                    .ThenInclude(q => q.EquipmentDefinition)
                    .ThenInclude(q => q.EquipmentType)
                    .ThenInclude(q => q.EquipmentDefinitions)
                    .ThenInclude(q => q.EquipmentSpecifications)
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

            // BEFREE: Single Reponsibility VIOLATED
            // Include Equipment Labels
            Expression<Func<EquipmentAllocation, bool>> eqIncludeLabelsExp = null;
            string equipmentLbl = Enum.GetName(EquipmentLabel.Equipment);
            string componentLbl = Enum.GetName(EquipmentLabel.Component);
            string partLbl = Enum.GetName(EquipmentLabel.Part);

            bool includeEquipment = filter.IncludeLabels.Contains(equipmentLbl);
            bool includeComponent = filter.IncludeLabels.Contains(componentLbl);
            bool includePart = filter.IncludeLabels.Contains(partLbl);

            // If labels = Equipment, Component and Part => Skip this step
            if (!(includeEquipment && includeComponent && includePart))
            {
                // Only entities with parent types
                if (includeEquipment)
                    eqIncludeLabelsExp = eqIncludeLabelsExp.ConcatOr(q => q.Equipment.EquipmentDefinition.ParentID == null);

                // Entities of child definitions + are attached
                if (includeComponent)
                    eqIncludeLabelsExp = eqIncludeLabelsExp.ConcatOr(q => q.Equipment.EquipmentDefinition.ParentID != null && q.Equipment.ParentID != null);

                // Entities of child definitions + are detached
                if (includePart)
                    eqIncludeLabelsExp = eqIncludeLabelsExp.ConcatOr(q => q.Equipment.EquipmentDefinition.ParentID != null && q.Equipment.ParentID == null);
            }

            var finalWhereExp = ExpressionCombiner.And(
            ignoreArchived,
            eqOfRoomsExp,
            eqOfPersonnelExp,
            eqOfTypeExp,
            eqOfDefinitionsExp,
            eqUsingStateExp,
            eqFunctionalityStateExp,
            eqIncludeLabelsExp);

            return finalWhereExp;
        }
    }
}
