using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Factories.LogFactories;
using temsAPI.Services;
using temsAPI.ViewModels;
using temsAPI.ViewModels.Room;

namespace temsAPI.Data.Managers
{
    public class RoomManager : EntityManager
    {
        LogManager _logManager;

        public RoomManager(
            IUnitOfWork unitOfWork, 
            ClaimsPrincipal user,
            LogManager logManager) : base(unitOfWork, user)
        {
            _logManager = logManager;
        }

        public async Task<string> Create(AddRoomViewModel viewModel)
        {
            string validationResult = await viewModel.Validate(_unitOfWork);
            if (validationResult != null)
                return validationResult;

            Room model = new Room
            {
                Id = Guid.NewGuid().ToString(),
                Description = viewModel.Description,
                Floor = viewModel.Floor,
                Identifier = viewModel.Identifier,
            };

            List<string> labelIds = viewModel.Labels.Select(q => q.Value).ToList() ?? new();
            List<string> supervisoriesIds = viewModel.Supervisories.Select(q => q.Value).ToList() ?? new();

            await model.AssignLabels(labelIds, _unitOfWork);
            await model.AssignSupervisories(supervisoriesIds, _unitOfWork);

            await _unitOfWork.Rooms.Create(model);
            await _unitOfWork.Save();

            return null;
        }

        public async Task<string> Update(AddRoomViewModel viewModel)
        {
            string validationResult = await viewModel.Validate(_unitOfWork);
            if (validationResult != null)
                return validationResult;

            var room = await GetFullById(viewModel.Id);
            if (room == null)
                return "Invalid room id provided";

            room.Identifier = viewModel.Identifier;
            room.Floor = viewModel.Floor;
            room.Description = viewModel.Description;

            List<string> labelIds = viewModel.Labels.Select(q => q.Value).ToList();
            List<string> supervisoriesIds = viewModel.Supervisories.Select(q => q.Value).ToList() ?? new();

            await room.AssignLabels(labelIds, _unitOfWork);

            var supervisoriesBefore = room.Supervisories.ToList();
            await room.AssignSupervisories(supervisoriesIds, _unitOfWork);
            var supervisoriesAfter = room.Supervisories.ToList();

            if (supervisoriesBefore != supervisoriesAfter)
            {
                // Supervisories changed
                var supervisoriesUpdatedLog = new RoomSupervisoriesChangedLogFactory(room, IdentityService.GetUserId(_user)).Create();
                await _logManager.Create(supervisoriesUpdatedLog);
            }

            await _unitOfWork.Save();
            return null;
        }

        public async Task<string> Remove(string roomId)
        {
            var room = await GetById(roomId);
            if (room == null)
                return "Invalid id provided";

            return await Remove(room);
        }

        public async Task<string> Remove(Room room)
        {
            _unitOfWork.Rooms.Delete(room);
            await _unitOfWork.Save();
            return null;
        }

        public async Task<Room> GetById(string roomId)
        {
            var room = (await _unitOfWork.Rooms
                .Find<Room>(
                    where: q => q.Id == roomId,
                    include: q => q
                    .Include(q => q.Labels)
                    .Include(q => q.Supervisories)))
                .FirstOrDefault();

            return room;
        }

        public async Task<Room> GetFullById(string roomId)
        {
            var room = (await _unitOfWork.Rooms
                .Find<Room>(
                    where: q => q.Id == roomId,
                    include: q => q
                    .Include(q => q.Labels)
                    .Include(q => q.Supervisories)
                    .ThenInclude(q => q.Positions)
                    .Include(q => q.Tickets)))
                .FirstOrDefault();

            return room;
        }

        public async Task<List<Option>> GetAutocompleteOptions(string filter)
        {
            int take = int.MaxValue;
            Expression<Func<Room, bool>> expression = q => !q.IsArchieved;
            if (filter != null)
            {
                expression = q => !q.IsArchieved && q.Identifier.Contains(filter);
                take = 5;
            }

            var options = (await _unitOfWork.Rooms.FindAll<Option>(
                where: expression,
                take: take,
                orderBy: q => q.OrderBy(q => q.Identifier),
                select: q => new Option
                {
                    Value = q.Id,
                    Label = q.Identifier,
                    Additional = q.Description
                })).ToList();

            return options;
        }

        public async Task<List<ViewRoomSimplifiedViewModel>> GetRoomsSimplified(
            int skip = 0,
            int take = int.MaxValue)
        {
            var rooms = (await _unitOfWork.Rooms
                .FindAll<ViewRoomSimplifiedViewModel>(
                    where: q => !q.IsArchieved,
                    skip: skip,
                    take: take,
                    include: q => q
                    .Include(q => q.Labels)
                    .Include(q => q.EquipmentAllocations)
                    .Include(q => q.Tickets.Where(q => q.DateClosed == null)),
                    select: q => ViewRoomSimplifiedViewModel.FromModel(q),
                    orderBy: q => q.OrderBy(q => q.Identifier)))
                .ToList();

            return rooms;
        }

        public async Task<List<Option>> GetLabelOptions()
        {
            var options = (await _unitOfWork.RoomLabels
                .FindAll<Option>(
                    where: q => !q.IsArchieved,
                    select: q => new Option
                    {
                        Value = q.Id,
                        Label = q.Name
                    }))
                .ToList();

            return options;
        }
    }
}
