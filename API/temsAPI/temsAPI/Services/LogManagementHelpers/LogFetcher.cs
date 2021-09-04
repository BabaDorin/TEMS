using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Helpers;
using temsAPI.Helpers.Filters;
using temsAPI.Helpers.ReusableSnippets;

namespace temsAPI.Services.LogManagementHelpers
{
    public class LogFetcher : IFetcher<Log, LogFilter>
    {
        readonly IUnitOfWork _unitOfWork;

        public LogFetcher(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Log>> Fetch(LogFilter filter)
        {
            var whereExp = GetWhereExp(filter);

            Func<IQueryable<Log>, IIncludableQueryable<Log, object>> includeExp =
                q => q.Include(q => q.Equipment)
                     .Include(q => q.Room)
                     .Include(q => q.Personnel);

            Func<IQueryable<Log>, IOrderedQueryable<Log>> orderByExp = 
                q => q.OrderByDescending(q => q.DateCreated);

            return await _unitOfWork.Logs
                .FindAll<Log>(
                    where: whereExp,
                    include: includeExp,
                    orderBy: orderByExp
                );
        }

        private Expression<Func<Log, bool>> GetWhereExp(LogFilter filter)
        {
            // Filter by entity
            Expression<Func<Log, bool>> logsOfEntityExp = null;

            if (!String.IsNullOrEmpty(filter.EquipmentId))
                logsOfEntityExp = q => q.EquipmentID == filter.EquipmentId;

            if (!String.IsNullOrEmpty(filter.RoomId))
                logsOfEntityExp = q => q.RoomID== filter.RoomId;

            if (!String.IsNullOrEmpty(filter.PersonnelId))
                logsOfEntityExp = q => q.RoomID == filter.PersonnelId;

            // Filter by Equipment label (associate log with equipment label?????) or get label via join
            Expression<Func<Log, bool>> logsOfEquipmentLabelsExp = null;

            if (!filter.IncludeLabels.IsNullOrEmpty())
                logsOfEquipmentLabelsExp = q => filter.IncludeLabels.Contains(q.Equipment.Label);

            // Filter by Equipment label (associate log with equipment label?????) or get label via join
            Expression<Func<Log, bool>> logsBySearchExp = null;

            if (!String.IsNullOrEmpty(filter.SearchValue))
                logsBySearchExp = q => q.Description.ToLower().Contains(filter.SearchValue.ToLower());


            var finalExp = ExpressionCombiner.And(
                    logsOfEntityExp,
                    logsOfEquipmentLabelsExp,
                    logsBySearchExp);

            return finalExp;
        }

        public async Task<int> GetAmount(LogFilter filter)
        {
            var whereExp = GetWhereExp(filter);
            Func<IQueryable<Log>, IIncludableQueryable<Log, object>> includeExp =
                q => q.Include(q => q.Equipment);

            if (includeExp == null)
                await _unitOfWork.Logs.Count(whereExp);

            return includeExp == null
                ? await _unitOfWork.Logs.Count(whereExp)
                : await _unitOfWork.Logs.Count(
                        where: whereExp,
                        include: includeExp);
        }
    }
}
