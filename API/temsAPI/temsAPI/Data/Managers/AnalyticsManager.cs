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
using temsAPI.Services;
using temsAPI.System_Files;

namespace temsAPI.Data.Managers
{
    public class AnalyticsManager : EntityManager
    {
        private EquipmentManager _equipmentManager;
        private CurrencyConvertor _currencyConvertor;

        public AnalyticsManager(
            IUnitOfWork unitOfWork, 
            ClaimsPrincipal user,
            EquipmentManager equipmentManager,
            CurrencyConvertor currencyConvertor) : base(unitOfWork, user)
        {
            _equipmentManager = equipmentManager;
            _currencyConvertor = currencyConvertor;
        }

        public async Task<int> GetEquipmentAmount(
            string entityType = null,
            string entityId = null)
        {
            Expression<Func<Equipment, bool>> filterByEntityExpression =
                _equipmentManager.Eq_FilterByEntity(entityType, entityId);

            return await _unitOfWork.Equipments.Count(filterByEntityExpression);
        }

        public async Task<double> GetEquipmentTotalCost(
            string entityType = null,
            string entityId = null)
        {
            Expression<Func<Equipment, bool>> filterByEntityExpression =
                _equipmentManager.Eq_FilterByEntity(entityType, entityId);

            double sum = (double)(await _unitOfWork.Equipments
                .FindAll(
                    where: filterByEntityExpression,
                    select: q => _equipmentManager.GetEquipmentPriceInLei(q)
                )).Sum();

            return sum;
        }
    }
}
