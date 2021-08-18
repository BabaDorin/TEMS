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

            // OnlyParents (aka Parent inclusion)
            Expression<Func<Equipment, bool>> parentInclusionExp = null;
            if (filter.OnlyParents)
                parentInclusionExp = q => q.EquipmentDefinition.ParentID == null;

            // OnlyDetached
            Expression<Func<Equipment, bool>> onlyDetachedExp = null;
            if (filter.OnlyDetached)
                onlyDetachedExp = q => q.ParentID == null;

            // Place for more where filters

            // WHERE
            var finalWhereExp = ExpressionCombiner.And(
                ignoreArchived, 
                eqOfTypeExp,
                eqOfDefinitionsExp,
                parentInclusionExp,
                onlyDetachedExp);

            // INCLUDE
            Func<IQueryable<Equipment>, IIncludableQueryable<Equipment, object>> finalIncludeExp =
                q => q.Include(q => q.EquipmentDefinition).ThenInclude(q => q.EquipmentType)
                    .Include(q => q.EquipmentAllocations.Where(q => q.DateReturned == null))
                    .ThenInclude(q => q.Room)
                    .Include(q => q.EquipmentAllocations.Where(q => q.DateReturned == null))
                    .ThenInclude(q => q.Personnel);

            return await _unitOfWork.Equipments
                .FindAll<Equipment>(
                    skip: filter.Skip,
                    take: filter.Take,
                    include: finalIncludeExp,
                    where: finalWhereExp
                );
        }
    }
}
