using OfficeOpenXml.FormulaParsing.ExpressionGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Helpers;
using temsAPI.System_Files;

namespace temsAPI.Data.Managers
{
    public class AnalyticsManager : EntityManager
    {
        public AnalyticsManager(IUnitOfWork unitOfWork, ClaimsPrincipal user) : base(unitOfWork, user)
        {
        }

        public async Task<int> GetEquipmentAmount(
            string entityType = null,
            string entityId = null)
        {
            Expression<Func<Equipment, bool>> expression = q => !q.IsArchieved;
            
            if(entityType != null)
            {
                entityType = entityType.ToLower();
                if (entityType != "equipment" 
                    && HardCodedValues.EntityTypes.Contains(entityType)
                    && entityId != null)
                {
                    Expression<Func<Equipment, bool>> secondaryExpression = null;
                    switch (entityType)
                    {
                        case "room":
                            secondaryExpression = q 
                                => q.ActiveAllocation != null 
                                && q.ActiveAllocation.RoomID == entityId;
                            break;
                        case "personnel":
                            {
                                secondaryExpression = q
                                => q.ActiveAllocation != null
                                && q.ActiveAllocation.RoomID == entityId;
                            }
                            break;
                    }

                    expression = ExpressionCombiner.CombineTwo(expression, secondaryExpression);
                }
            }

            return await _unitOfWork.Equipments.Count(expression);
        }
    }
}
