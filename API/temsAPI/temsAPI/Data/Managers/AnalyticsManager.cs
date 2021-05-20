﻿using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using OfficeOpenXml.FormulaParsing.ExpressionGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Helpers;
using temsAPI.Helpers.AnalyticsHelpers.AnalyticsModels;
using temsAPI.Services;
using temsAPI.System_Files;

namespace temsAPI.Data.Managers
{
    public class AnalyticsManager : EntityManager
    {
        private EquipmentManager _equipmentManager;
        private TicketManager _ticketManager;
        private CurrencyConvertor _currencyConvertor;

        public AnalyticsManager(
            IUnitOfWork unitOfWork, 
            ClaimsPrincipal user,
            EquipmentManager equipmentManager,
            CurrencyConvertor currencyConvertor,
            TicketManager ticketManager) : base(unitOfWork, user)
        {
            _equipmentManager = equipmentManager;
            _currencyConvertor = currencyConvertor;
            _ticketManager = ticketManager;
        }

        // ------------------< Equipment >--------------------

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

        public async Task<PieChartData> GetEquipmentWorkabilityRate(string entityType, string entityId)
        {
            Expression<Func<Equipment, bool>> filterByEntityExpression =
                _equipmentManager.Eq_FilterByEntity(entityType, entityId);

            int working = await _unitOfWork.Equipments.Count(
               ExpressionCombiner.CombineTwo(filterByEntityExpression, q => !q.IsDefect));

            int defect = await _unitOfWork.Equipments.Count(
                ExpressionCombiner.CombineTwo(filterByEntityExpression, q => q.IsDefect));

            PieChartData pieChart = new()
            {
                ChartName = "Equipment workability rate",
                Rates = new List<Tuple<string, int>>()
                {
                    new Tuple<string, int>("Working", working),
                    new Tuple<string, int>("Defect", defect),
                }
            };

            return pieChart;
        }

        public async Task<PieChartData> GetEquipmentAllocationRate(
            string entityType = null,
            string entityId = null)
        {
            Expression<Func<Equipment, bool>> filterByEntityExpression =
                _equipmentManager.Eq_FilterByEntity(entityType, entityId);

            var rates = (await _unitOfWork.Equipments
                .FindAll<Equipment>(
                    where: filterByEntityExpression,
                    include: q => q.Include(q => q.EquipmentAllocations)
                    ))
                .GroupBy(q => q.ActiveAllocation == null)
                .Select(q => new Tuple<string, int>(q.Key ? "Allocated" : "unallocated", q.Count()))
                .ToList();

            PieChartData pieChart = new()
            {
                ChartName = "Equipment allocation rate",
                Rates = rates
            };

            return pieChart;
        }


        // ------------------< Ticket >--------------------

        public async Task<PieChartData> GetTicketClosingRate(
            string entityType = null,
            string entityId = null)
        {
            Expression<Func<Ticket, bool>> filterByEntityExpression =
                _ticketManager.Eq_FilterByEntity(entityType, entityId);

            var openTickets = (await _unitOfWork.Tickets.Count(
                ExpressionCombiner.CombineTwo(filterByEntityExpression, q => q.DateClosed == null)));
            
            var closedTickets = (await _unitOfWork.Tickets.Count(
                ExpressionCombiner.CombineTwo(filterByEntityExpression, q => q.DateClosed != null)));

            PieChartData pieChart = new()
            {
                ChartName = "Ticket closing rate",
                Rates = new List<Tuple<string, int>>()
                {
                    new Tuple<string, int>("Open: ", openTickets),
                    new Tuple<string, int>("Closed: ", closedTickets),
                }
            };

            return pieChart;
        }

        public async Task<PieChartData> GetTicketClosingByRate(
            string entityType = null,
            string entityId = null)
        {
            Expression<Func<Ticket, bool>> filterByEntityExpression =
                _ticketManager.Eq_FilterByEntity(entityType, entityId);

            Expression<Func<Ticket, bool>> finalExpression =
                ExpressionCombiner.CombineTwo(
                    filterByEntityExpression,
                    q => q.DateClosed != null);

            var rates = (await _unitOfWork.Tickets
                .FindAll<Ticket>(
                    include: q => q.Include(q => q.ClosedBy),
                    where: finalExpression))
                .GroupBy(q => q.ClosedBy?.FullName ?? q.ClosedBy?.UserName)
                .Select(q => new Tuple<string, int>(q.Key, q.Count()))
                .ToList();

            PieChartData pieChart = new()
            {
                ChartName = "Ticket closing by user rate",
                Rates = rates
            };

            return pieChart;
        }

        public async Task<PieChartData> GetOpenTicketStatusRate(
            string entityType = null,
            string entityId = null)
        {
            Expression<Func<Ticket, bool>> filterByEntityExpression =
                _ticketManager.Eq_FilterByEntity(entityType, entityId);

            var rates = (await _unitOfWork.Tickets
                .FindAll<Ticket>(
                    include: q => q.Include(q => q.Status),
                    where: filterByEntityExpression))
                .GroupBy(q => q.Status.Name)
                .Select(q => new Tuple<string, int>(q.Key, q.Count()))
                .ToList();

            PieChartData pieChart = new()
            {
                ChartName = "Status rate of open tickets",
                Rates = rates
            };

            return pieChart;
        }

        // ------------------< User >--------------------

        public async Task<double> GetAmountOfCreatedTickets(
            string entityType = null,
            string entityId = null)
        {
            Expression<Func<Ticket, bool>> filterByEntityExpression =
                _ticketManager.Eq_FilterByEntity(entityType, entityId, TicketManager.UserTicketAction.Create);

            var amount = (await _unitOfWork.Tickets.Count(filterByEntityExpression));
            return amount;
        }

        public async Task<double> GetAmountOfClosedTickets(
            string entityType = null,
            string entityId = null)
        {
            Expression<Func<Ticket, bool>> filterByEntityExpression =
                _ticketManager.Eq_FilterByEntity(entityType, entityId, TicketManager.UserTicketAction.Close);

            var finalExpression = ExpressionCombiner.CombineTwo(
                filterByEntityExpression,
                q => q.DateClosed != null);

            var amount = (await _unitOfWork.Tickets.Count(finalExpression));
            return amount;
        }

        public async Task<double> GetAmountOfOpenTickets(
            string entityType = null,
            string entityId = null)
        {
            Expression<Func<Ticket, bool>> filterByEntityExpression =
                _ticketManager.Eq_FilterByEntity(entityType, entityId);

            var finalExpression = ExpressionCombiner.CombineTwo(
                filterByEntityExpression,
                q => q.DateClosed == null);

            var amount = (await _unitOfWork.Tickets.Count(finalExpression));
            return amount;
        }

        public async Task<PieChartData> GetAmountOfLastCreatedTickets(DateTime start, DateTime end, string interval)
        {
            //Expression<Func<Ticket, Ticket>> exp = null;

            var ticketGroups = (await _unitOfWork.Tickets
                .FindAll<Ticket>(
                    where: q => !q.IsArchieved && q.DateCreated >= start && q.DateClosed <= end,
                    orderBy: q => q.OrderByDescending(q => q.DateCreated)))
                .GroupBy(q =>
                (interval == "daily")
                ? q.DateCreated.Date
                : new DateTime(q.DateCreated.Year, q.DateCreated.Month, 1))
                .ToList();

            PieChartData chartData = new PieChartData();

            if(interval == "daily")
            {
                chartData.ChartName = "Number of tickets created in the last days";
                for (var day = start.Date; day.Date <= end.Date; day = day.AddDays(1))
                {
                    var group = ticketGroups.FirstOrDefault(q => q.Key == day);
                    chartData.Rates.Add(new Tuple<string, int>(
                        day.Date.ToString(),
                        (group == null) ? 0 : group.Count()));
                }
            }

            if (interval == "monthly")
            {
                chartData.ChartName = "Number of tickets created in the last months";
                for (var month = start.Date; month.Date <= end.Date; month = month.AddMonths(1))
                {
                    var group = ticketGroups.FirstOrDefault(q => q.Key == new DateTime(
                        month.Year, month.Month, 1));

                    chartData.Rates.Add(new Tuple<string, int>(
                        month.Date.ToString("yyyy/MMM"),
                        group?.Count() ?? 0));
                }
            }

            return chartData;
        }

        // very good indicator btw =) An user might superficially close many tickets,
        // and if the problem has not been fully solved, those specific tickets will get reopened by 
        // someone else or by the user himself afterwards.
        public async Task<int> GetAmountOfTicketsClosedByUserThatWereReopenedAfterwards(string userId)
        {
            var amount = (await _unitOfWork.TEMSUsers
                .Find<int>(
                    where: q => q.Id == userId,
                    include: q => q.Include(q => q.ClosedAndThenReopenedTickets),
                    select: q => q.ClosedAndThenReopenedTickets.Count()))
                .FirstOrDefault();

            return amount;
        }

        // Get the amount of tickets ever closed by the specified user, including
        // the one that were reopened afterwards
        public async Task<int> GetAmountOfTicketsEverClosedByUser(string userId)
        {
            var user = (await _unitOfWork.TEMSUsers
                .Find<TEMSUser>(
                    where: q => q.Id == userId,
                    include: q => q
                    .Include(q => q.ClosedTickets)
                    .Include(q => q.ClosedAndThenReopenedTickets)
                )).FirstOrDefault();

            if (user == null)
                throw new Exception("Invalid id provided");

            var amount = user.ClosedTickets.Union(user.ClosedAndThenReopenedTickets).Count();

            return amount;
        }
    }
}
