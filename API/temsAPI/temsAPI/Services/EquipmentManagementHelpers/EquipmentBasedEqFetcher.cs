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
    /// Fetches equipment from Equipments table (When no allocatee is specified).
    /// </summary>
    public class EquipmentBasedEqFetcher : IEquipmentFetcher
    {
        private IUnitOfWork _unitOfWork;

        public EquipmentBasedEqFetcher(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Equipment>> Fetch(EquipmentFilter filter)
        {
            Expression<Func<Equipment, bool>> whereExp = GetWhereExp(filter);

            // INCLUDE
            Func<IQueryable<Equipment>, IIncludableQueryable<Equipment, object>> finalIncludeExp =
                q => q.Include(q => q.EquipmentDefinition)
                    .ThenInclude(q => q.EquipmentType)
                    .Include(q => q.EquipmentDefinition)
                    .ThenInclude(q => q.EquipmentSpecifications)
                    .Include(q => q.EquipmentAllocations.Where(q => q.DateReturned == null))
                    .ThenInclude(q => q.Room)
                    .Include(q => q.EquipmentAllocations.Where(q => q.DateReturned == null))
                    .ThenInclude(q => q.Personnel);

            return await _unitOfWork.Equipments
                .FindAll<Equipment>(
                    skip: filter.Skip,
                    take: filter.Take,
                    include: finalIncludeExp,
                    where: whereExp
                );
        }

        public async Task<int> GetAmount(EquipmentFilter filter)
        {
            Expression<Func<Equipment, bool>> whereExp = GetWhereExp(filter);

            Func<IQueryable<Equipment>, IIncludableQueryable<Equipment, object>> finalIncludeExp =
                q => q.Include(q => q.EquipmentDefinition)
                    .ThenInclude(q => q.EquipmentType);

            return await _unitOfWork.Equipments.Count(
                where: whereExp,
                include: finalIncludeExp);
        }

        private Expression<Func<Equipment, bool>> GetWhereExp(EquipmentFilter filter)
        {
            // Default filter
            Expression<Func<Equipment, bool>> ignoreArchived = q => !q.IsArchieved;

            // Type filtering
            Expression<Func<Equipment, bool>> eqOfTypeExp = null;
            if (!filter.Types.IsNullOrEmpty() && filter.Types.IndexOf("any") == -1)
                eqOfTypeExp = q => filter.Types.Contains(q.EquipmentDefinition.EquipmentTypeID);

            // Definition filtering
            Expression<Func<Equipment, bool>> eqOfDefinitionsExp = null;
            if (!filter.Definitions.IsNullOrEmpty() && filter.Definitions.IndexOf("any") == -1)
                eqOfDefinitionsExp = q => filter.Definitions.Contains(q.EquipmentDefinitionID);

            // Include In Use / Unused
            Expression<Func<Equipment, bool>> eqUsingStateExp = null;
            if (!(filter.IncludeInUse && filter.IncludeUnused))
            {
                eqUsingStateExp = q => q.IsUsed == filter.IncludeInUse;
            }

            // Include Functional / Defect
            Expression<Func<Equipment, bool>> eqFunctionalityStateExp = null;
            if (!(filter.IncludeFunctional && filter.IncludeDefect))
            {
                eqFunctionalityStateExp = q => q.IsDefect == !filter.IncludeFunctional;
            }

            // BEFREE: Single Reponsibility VIOLATED
            // Include Equipment Labels
            Expression<Func<Equipment, bool>> eqIncludeLabelsExp = null;
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
                    eqIncludeLabelsExp = eqIncludeLabelsExp.ConcatOr(q => q.EquipmentDefinition.ParentID == null);

                // Entities of child definitions + are attached
                if (includeComponent)
                    eqIncludeLabelsExp = eqIncludeLabelsExp.ConcatOr(q => q.EquipmentDefinition.ParentID != null && q.ParentID != null);

                // Entities of child definitions + are detached
                if (includePart)
                    eqIncludeLabelsExp = eqIncludeLabelsExp.ConcatOr(q => q.EquipmentDefinition.ParentID != null && q.ParentID == null);
            }

            // WHERE
            var finalWhereExp = ExpressionCombiner.And(
                ignoreArchived,
                eqOfTypeExp,
                eqOfDefinitionsExp,
                eqUsingStateExp,
                eqFunctionalityStateExp,
                eqIncludeLabelsExp);

            return finalWhereExp;
        }
    }
}
