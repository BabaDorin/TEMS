using OfficeOpenXml.FormulaParsing.ExpressionGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Helpers.EquipmentHelpers;
using temsAPI.Helpers.Filters;
using temsAPI.Helpers.ReusableSnippets;

namespace temsAPI.Services.AllocationManagementHelpers
{
    public class AllocationFetcher : IFetcher<EquipmentAllocation, AllocationFilter>
    {
        public Task<IEnumerable<EquipmentAllocation>> Fetch(AllocationFilter filter)
        {
            // Build WHERE expression
            
            // Of Equipment
            Expression<Func<EquipmentAllocation, bool>> alOfEquipmentExp = null;
            if (!filter.Equipment.IsNullOrEmpty())
                alOfEquipmentExp = q => filter.Equipment.Contains(q.Id);

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
            alOfLabels = q => filter.IncludeLabels.Contains(q.Label);

            // Include statuses 
            Expression<Func<EquipmentAllocation, bool>> alOfStatuses = nuvpll;
            alOfStatuses = q => q.DateAllocated
        }

        public Task<int> GetAmount(AllocationFilter filter)
        {
            throw new NotImplementedException();
        }
    }
}
