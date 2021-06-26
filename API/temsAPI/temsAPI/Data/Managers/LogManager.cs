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
using temsAPI.Services;
using temsAPI.ViewModels;
using temsAPI.ViewModels.Log;

namespace temsAPI.Data.Managers
{
    public class LogManager : EntityManager
    {
        private IdentityService _identityService;

        public LogManager(
            IUnitOfWork unitOfWork, 
            ClaimsPrincipal user,
            IdentityService identityService) : base(unitOfWork, user)
        {
            _identityService = identityService;
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
                    CreatedByID = _identityService.GetUserId(),
                    Description = viewModel.Description,
                };

                switch (viewModel.AddresseesType)
                {
                    case "equipment": log.EquipmentID = addressee.Value; break;
                    case "room": log.RoomID = addressee.Value; break;
                    case "personnel": log.PersonnelID = addressee.Value; break;
                }

                await Create(log);
            }

            return null;
        }

        public async Task Create(Log log)
        {
            await _unitOfWork.Logs.Create(log);
            await _unitOfWork.Save();
        }

        public async Task<string> Remove(string logId)
        {
            var log = await GetById(logId);
            if (log == null)
                return "Invalid id provided";

            _unitOfWork.Logs.Delete(log);
            await _unitOfWork.Save();
            return null;
        }

        public async Task<Log> GetById(string logId)
        {
            return (await _unitOfWork.Logs
                .Find<Log>(q => q.Id == logId))
                .FirstOrDefault();
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
                    .Include(q => q.CreatedBy),
                    orderBy: q => q.OrderByDescending(q => q.DateCreated),
                    skip: skip,
                    take: take,
                    select: q => ViewLogViewModel.FromModel(q)
                )).ToList();

            return logs;
        }
    }
}
