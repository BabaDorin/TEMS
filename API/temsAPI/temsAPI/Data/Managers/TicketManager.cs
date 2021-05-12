using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Helpers;
using temsAPI.Services;
using temsAPI.System_Files;
using temsAPI.ViewModels;
using temsAPI.ViewModels.Ticket;

namespace temsAPI.Data.Managers
{
    public class TicketManager : EntityManager
    {
        private enum TicketEntityType
        {
            Any,
            UserClosed,
            UserCreated,
            UserAssigned,
            Equipment,
            Room,
            Personnel
        }

        private enum TicketOrderByCriteria
        {
            Priority,
            Recency,
            RecencyClosed
        }

        public enum UserTicketAction
        {
            Create,
            Close,
            Assigned
        }

        private UserManager<TEMSUser> _userManager;
        private IdentityService _identityService;

        public TicketManager(
            IUnitOfWork unitOfWork, 
            ClaimsPrincipal user,
            UserManager<TEMSUser> userManager,
            IdentityService identityService) : base(unitOfWork, user)
        {
            _userManager = userManager;
            _identityService = identityService;
        }

        // BEFREE
        public async Task<List<ViewTicketSimplifiedViewModel>> GetEntityTickets(
            string entityType,
            string entityId,
            bool includingClosed,
            bool onlyClosed,
            string orderBy = null,
            int skip = 0,
            int take = int.MaxValue)
        {
            // Invalid IdentityType
            if ((new List<string> { "any", "user closed", "user created", "user assigned", "equipment", "room", "personnel" }).IndexOf(entityType) == -1)
                throw new Exception("Invalid type or id provided");

            // No identityId Provided
            if (String.IsNullOrEmpty(entityId.Trim()))
                throw new Exception($"You have to provide a valid {entityType} Id");

            // Checking if identityId is valid and at the same time we build the expression
            // false false
            Expression<Func<Ticket, bool>> expression = q => q.DateClosed == null && !q.IsArchieved;

            // true false
            if (includingClosed)
                expression = q => !q.IsArchieved;

            // any true
            if (onlyClosed)
                expression = q => q.DateClosed.HasValue && !q.IsArchieved;

            Expression<Func<Ticket, bool>> expression2 = null;

            switch (entityType)
            {
                case "user closed":
                    if (!await _unitOfWork.TEMSUsers.isExists(q => q.Id == entityId))
                        throw new Exception("Invalid type or id provided");

                    expression2 = q => q.ClosedById == entityId;
                    break;
                case "user created":
                    if (!await _unitOfWork.TEMSUsers.isExists(q => q.Id == entityId))
                        throw new Exception("Invalid type or id provided");

                    expression2 = q => q.CreatedById == entityId;
                    break;
                case "user assigned":
                    var user = await _userManager.FindByIdAsync(entityId);
                    if (user == null)
                        throw new Exception("Invalid type or id provided");

                    expression2 = q => q.Assignees.Contains(user);
                    break;
                case "equipment":
                    if (!await _unitOfWork.Equipments.isExists(q => q.Id == entityId))
                        throw new Exception("Invalid type or id provided");

                    expression2 = q => q.Equipments.Any(q => q.Id == entityId);
                    break;
                case "room":
                    if (!await _unitOfWork.Rooms.isExists(q => q.Id == entityId))
                        throw new Exception("Invalid type or id provided");

                    expression2 = q => q.Rooms.Any(q => q.Id == entityId);
                    break;
                case "personnel":
                    if (!await _unitOfWork.Personnel.isExists(q => q.Id == entityId))
                        throw new Exception("Invalid type or id provided");

                    expression2 = q => q.Personnel.Any(q => q.Id == entityId);
                    break;
            }

            expression = ExpressionCombiner.CombineTwo(expression, expression2);

            Func<IQueryable<Ticket>, IOrderedQueryable<Ticket>> orderByExpression = null;
            switch (orderBy)
            {
                case "recency":
                    orderByExpression = q => q.OrderByDescending(q => q.Status.ImportanceIndex);
                    break;
                case "recency closed":
                    orderByExpression = q => q.OrderByDescending(q => q.DateClosed);
                    break;
                default:
                    orderByExpression = q => q.OrderByDescending(q => q.DateCreated);
                    break;
            }

            var tickets = (await _unitOfWork.Tickets
                .FindAll<ViewTicketSimplifiedViewModel>(
                    include: q => q
                    .Include(q => q.Assignees)
                    .Include(q => q.ClosedBy)
                    .Include(q => q.CreatedBy)
                    .Include(q => q.Label)
                    .Include(q => q.Personnel)
                    .Include(q => q.Rooms)
                    .Include(q => q.Status)
                    .Include(q => q.Equipments),
                    where: expression,
                    orderBy: orderByExpression,
                    skip: skip,
                    take: take,
                    select: q => ViewTicketSimplifiedViewModel.FromModel(q)
                )).ToList();

            return tickets;
        }

        public async Task<Ticket> GetById(string ticketId)
        {
            var ticket = (await _unitOfWork.Tickets
                .Find<Ticket>(q => q.Id == ticketId))
                .FirstOrDefault();

            return ticket;
        }

        public async Task CloseTicket(Ticket ticket)
        {
            ticket.ClosedById = _identityService.GetUserId();
            ticket.DateClosed = DateTime.Now;
            await _unitOfWork.Save();
        }

        public async Task<string> Create(AddTicketViewModel viewModel)
        {
            string validationResult = await viewModel.Validate(_unitOfWork);
            if (validationResult != null)
                return validationResult;
            
            Ticket model = new Ticket
            {
                Id = Guid.NewGuid().ToString(),
                StatusId = viewModel.Status,
                Problem = viewModel.Problem,
                CreatedBy = (_identityService.IsAuthenticated())
                    ? await _userManager.FindByIdAsync(_identityService.GetUserId())
                    : null,
                DateCreated = DateTime.Now,
                Description = viewModel.ProblemDescription,
                Rooms = await _unitOfWork.Rooms.FindAll<Room>(
                    where: q =>
                        viewModel.Rooms.Select(q => q.Value).Contains(q.Id)),
                Equipments = await _unitOfWork.Equipments.FindAll<Equipment>(
                    where: q =>
                        viewModel.Equipments.Select(q => q.Value).Contains(q.Id)),
                Personnel = await _unitOfWork.Personnel.FindAll<Personnel>(
                    where: q =>
                        viewModel.Personnel.Select(q => q.Value).Contains(q.Id)),
                Assignees = await _unitOfWork.TEMSUsers.FindAll<TEMSUser>(
                    where: q =>
                        viewModel.Assignees.Select(q => q.Value).Contains(q.Id)),
            };

            await _unitOfWork.Tickets.Create(model);
            await _unitOfWork.Save();

            return null;
        }

        public async Task ChangeTicketStatus(Ticket ticket, Status newStatus)
        {
            ticket.Status = newStatus;
            await _unitOfWork.Save();
        }

        public async Task<List<Option>> GetTicketStatuses()
        {
            var statuses = (await _unitOfWork
                .Statuses
                .FindAll<Option>(
                    where: q => !q.IsArchieved,
                    select: q => new Option
                    {
                        Value = q.Id,
                        Label = q.Name
                    }))
                .ToList();

            return statuses;
        }

        public async Task<Status> GetTicketStatusByStatusId(string statusId)
        {
            var status = (await _unitOfWork.Statuses
                .Find<Status>(q => q.Id == statusId))
                .FirstOrDefault();

            return status;
        }
        
        public async Task ReopenTicket(Ticket ticket)
        {
            var previouslyClosedBy = (await _unitOfWork.TEMSUsers
                .Find<TEMSUser>(q => q.Id == ticket.ClosedById))
                .FirstOrDefault();

            if (previouslyClosedBy != null)
            {
                ticket.PreviouslyClosedBy.Remove(previouslyClosedBy);
                ticket.PreviouslyClosedBy.Add(previouslyClosedBy);
            }

            ticket.ClosedById = null;
            ticket.DateClosed = null;
            await _unitOfWork.Save();
        }

        // BEFREE
        public async Task<List<ViewTicketSimplifiedViewModel>> GetTickets(
            string equipmentId,
            string roomId,
            string personnelId,
            bool includingClosed,
            bool onlyClosed)
        {
            // Invalid equipmentId
            if (equipmentId != "any" && !await _unitOfWork.Equipments.isExists(q => q.Id == equipmentId))
                throw new Exception("Invalid equipment provided");

            // Invalid roomId provided
            if (roomId != "any" && !await _unitOfWork.Rooms.isExists(q => q.Id == roomId))
                throw new Exception("Invalid room provided");

            // Invalid personnelId provided
            if (personnelId != "any" && !await _unitOfWork.Personnel.isExists(q => q.Id == personnelId))
                throw new Exception("Invalid personnel provided");

            // false false
            Expression<Func<Ticket, bool>> closedExpression = q => q.DateClosed == null && !q.IsArchieved;

            // true false
            if (includingClosed)
                closedExpression = q => !q.IsArchieved;

            // any true
            if (onlyClosed)
                closedExpression = q => q.DateClosed.HasValue && !q.IsArchieved;

            Expression<Func<Ticket, bool>> equipmentExpression = (equipmentId == "any")
               ? null
               : q => q.Equipments.Any(q => q.Id == equipmentId);

            Expression<Func<Ticket, bool>> roomExpression = (roomId == "any")
               ? null
               : q => q.Rooms.Any(q => q.Id == roomId);

            Expression<Func<Ticket, bool>> personnelExpression = (personnelId == "any")
               ? null
               : q => q.Personnel.Any(q => q.Id == personnelId);

            var finalExpression = ExpressionCombiner.And(closedExpression, equipmentExpression, roomExpression, personnelExpression);

            var tickets = (await _unitOfWork.Tickets
                .FindAll<ViewTicketSimplifiedViewModel>(
                    where: finalExpression,
                    include: q => q.Include(q => q.Assignees)
                                   .Include(q => q.ClosedBy)
                                   .Include(q => q.CreatedBy)
                                   .Include(q => q.Label)
                                   .Include(q => q.Personnel)
                                   .Include(q => q.Rooms)
                                   .Include(q => q.Status)
                                   .Include(q => q.Equipments)
                                   .Include(q => q.Assignees),
                    orderBy: q => q.OrderBy(q => q.Status.ImportanceIndex)
                                   .ThenByDescending(q => q.DateCreated),
                    select: q => ViewTicketSimplifiedViewModel.FromModel(q)
                )).ToList();

            return tickets;
        }

        public Expression<Func<Ticket, bool>> Eq_FilterByEntity(
            string entityType, 
            string entityId, 
            UserTicketAction? userTicketAction = null)
        {
            Expression<Func<Ticket, bool>> expression = q => !q.IsArchieved;
            if (entityType == null)
                return expression;

            entityType = entityType.ToLower();
            if (!HardCodedValues.EntityTypes.Contains(entityType) || entityId == null)
                return expression;

            Expression<Func<Ticket, bool>> secondaryExpression = null;
            switch (entityType)
            {
                case "room":
                    secondaryExpression = q
                        => q.Rooms.Any(q => q.Id == entityId);
                    break;
                case "personnel":
                    secondaryExpression = q
                        => q.Personnel.Any(q => q.Id == entityId);
                    break;
                case "equipment":
                    secondaryExpression = q
                        => q.Equipments.Any(q => q.Id == entityId);
                    break;
                case "user":
                    if (userTicketAction == null)
                        secondaryExpression = null;

                    if(userTicketAction == UserTicketAction.Create)
                        secondaryExpression = q => q.CreatedById == entityId;

                    if (userTicketAction == UserTicketAction.Close)
                        secondaryExpression = q => q.ClosedById == entityId;
                    break;
            }

            expression = ExpressionCombiner.CombineTwo(expression, secondaryExpression);
            return expression;
        }
    }
}
