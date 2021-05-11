using Microsoft.EntityFrameworkCore;
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
using temsAPI.Helpers.AnalyticsHelpers.AnalyticsModels;
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

        public async Task<PieChartData> GetEquipmentUtilizationRate(
            string entityType, 
            string entityId)
        {
            Expression<Func<Equipment, bool>> filterByEntityExpression =
                _equipmentManager.Eq_FilterByEntity(entityType, entityId);

            int currentlyInUse = await _unitOfWork.Equipments.Count(
                ExpressionCombiner.CombineTwo(filterByEntityExpression, q => q.IsUsed));

            int currentlyUnengaged = await _unitOfWork.Equipments.Count(
                ExpressionCombiner.CombineTwo(filterByEntityExpression, q => !q.IsUsed));


            var pieChart = new PieChartData
            {
                ChartName = "Equipment utilization rate",
                Rates = new List<Tuple<string, int>>()
                {
                    new Tuple<string, int>("Currently in use", currentlyInUse),
                    new Tuple<string, int>("Unengaged", currentlyUnengaged),
                }
            };

            return pieChart;
        }

        public async Task<PieChartData> GetEquipmentTypeRate(
            string entityType,
            string entityId)
        {
            Expression<Func<Equipment, bool>> filterByEntityExpression =
                _equipmentManager.Eq_FilterByEntity(entityType, entityId);


            var rates = (await _unitOfWork.Equipments
                .FindAll<Equipment>(
                    where: filterByEntityExpression,
                    include: q => q.Include(q => q.EquipmentDefinition).ThenInclude(q => q.EquipmentType)
                    ))
                .GroupBy(q => q.EquipmentDefinition.EquipmentType.Name)
                .Select(q => new Tuple<string, int>(q.Key, q.Count()))
                .ToList();

            PieChartData pieChart = new()
            {
                ChartName = "Equipment type rates",
                Rates = rates
            };

            return pieChart;
        }
    }
}
