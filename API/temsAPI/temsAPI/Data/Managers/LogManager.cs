using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Helpers;
using temsAPI.ViewModels;
using temsAPI.ViewModels.Log;

namespace temsAPI.Data.Managers
{
    public class LogManager : EntityManager
    {
        public LogManager(IUnitOfWork unitOfWork, ClaimsPrincipal user) : base(unitOfWork, user)
        {
        }

        public async Task<string> Create(AddLogViewModel viewModel)
        {
            string validationResult = viewModel.Validate();
            if (validationResult != null)
                return validationResult;

            foreach (Option addressee in viewModel.Addressees)
            {
                Log log = new Log
                {
                    Id = Guid.NewGuid().ToString(),
                    DateCreated = DateTime.Now,
                    IsImportant = viewModel.IsImportant,
                    LogTypeID = viewModel.LogTypeId,
                    Text = viewModel.Text,
                };

                switch (viewModel.AddresseesType)
                {
                    case "equipment": log.EquipmentID = addressee.Value; break;
                    case "room": log.RoomID = addressee.Value; break;
                    case "personnel": log.PersonnelID = addressee.Value; break;
                }

                await _unitOfWork.Logs.Create(log);
                await _unitOfWork.Save();
            }

            return null;
        }

        public async Task<List<Option>> GetLogTypes()
        {
            var logTypes = (await _unitOfWork.LogTypes
                .FindAll<Option>(
                    select: q => new Option
                    {
                        Value = q.Id,
                        Label = q.Type
                    }
                )).ToList();

            return logTypes;
        }

        public async Task<List<ViewLogViewModel>> GetEntityLogs(
            string entityType, 
            string entityId, 
            int skip = 0, 
            int take = int.MaxValue)
        {
            if ((new List<string>() { "any", "equipment", "personnel", "room" }).IndexOf(entityType) == -1)
                throw new Exception($"{entityType} is not a valid tems type, valid: any, equipment, personnel, room");

            Expression<Func<Log, bool>> expression = q => !q.IsArchieved;
            Expression<Func<Log, bool>> secondaryExp = null;

            switch (entityType)
            {
                case "equipment":
                    secondaryExp = q => q.EquipmentID == entityId;
                    break;
                case "room":
                    secondaryExp = q => q.RoomID == entityId;
                    break;
                case "personnel":
                    secondaryExp = q => q.PersonnelID == entityId;
                    break;
            }

            expression = ExpressionCombiner.CombineTwo(expression, secondaryExp);

            var logs = (await _unitOfWork.Logs
                .FindAll<ViewLogViewModel>(
                    where: expression,
                    include: q => q
                    .Include(q => q.Equipment)
                    .Include(q => q.Personnel)
                    .Include(q => q.Room)
                    .Include(q => q.LogType),
                    orderBy: q => q.OrderByDescending(q => q.DateCreated),
                    skip: skip,
                    take: take,
                    select: q => ViewLogViewModel.FromModel(q)
                )).ToList();

            return logs;
        }
    }
}
