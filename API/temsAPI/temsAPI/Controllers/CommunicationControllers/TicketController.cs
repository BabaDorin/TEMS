using AutoMapper;
using AutoMapper.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.OpenApi.Any;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.ViewModels;
using temsAPI.ViewModels.Status;
using temsAPI.ViewModels.Ticket;

namespace temsAPI.Controllers.CommunicationControllers
{
    public class TicketController : TEMSController
    {
        public TicketController(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            UserManager<TEMSUser> userManager) : base(mapper, unitOfWork, userManager)
        {
        }

        [HttpGet("/ticket/getticketsofentity/{entityType}/{entityId}/{includingClosed}/{onlyClosed}")]
        public async Task<JsonResult> GetTicketsOfEntity(
            string entityType,
            string entityId,
            bool includingClosed,
            bool onlyClosed)
        {
            try
            {
                // Invalid IdentityType
                if ((new List<string> { "any", "equipment", "room", "personnel" }).IndexOf(entityType) == -1)
                    return ReturnResponse("Invalid identity type.", ResponseStatus.Fail);

                // No identityId Provided
                if (String.IsNullOrEmpty(entityId.Trim()))
                    return ReturnResponse($"You have to provide a valid {entityType} Id", ResponseStatus.Fail);

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
                    case "equipment":
                        expression2 = q =>  q.Equipments.AsEnumerable().Any(q => q.Id == entityId);
                        break;
                    case "room":
                        expression2 = q => q.Rooms.AsEnumerable().Any(q => q.Id == entityId);
                        break;
                    case "personnel":
                        expression2 = q => q.Personnel.AsEnumerable().Any(q => q.Id == entityId);
                        break;
                }

                List<ViewTicketSimplifiedViewModel> viewModel = (await _unitOfWork.Tickets
                    .FindAll<ViewTicketSimplifiedViewModel>(
                        where: ExpressionCombiner.And(expression, expression2),
                        include: q => q.Include(q => q.Assignees)
                                       .Include(q => q.ClosedBy)
                                       .Include(q => q.CreatedBy)
                                       .Include(q => q.Label)
                                       .Include(q => q.Personnel)
                                       .Include(q => q.Rooms)
                                       .Include(q => q.Status)
                                       .Include(q => q.Equipments),
                        orderBy: q => q.OrderBy(q => q.Status.ImportanceIndex),
                        select: q => new ViewTicketSimplifiedViewModel
                        {
                            Id = q.Id,
                            DateClosed = q.DateClosed,
                            DateCreated = q.DateCreated,
                            Description = q.Description,
                            Equipments = q.Equipments.Select(q => new Option
                            {
                                Value = q.Id,
                                Label = q.TemsIdOrSerialNumber
                            }).ToList(),
                            Label = new Option
                            {
                                Value = q.Label.Id,
                                Label = q.Label.Name,
                                Additional = q.Label.ColorHex
                            },
                            Personnel = q.Personnel.Select(q => new Option
                            {
                                Value = q.Id,
                                Label = q.Name
                            }).ToList(),
                            Problem = q.Problem,
                            Status = new Option
                            {
                                Value = q.Status.Id,
                                Label = q.Status.Name,
                            }
                        }
                    )).ToList();

                return (Json(viewModel));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching issues", ResponseStatus.Fail);
            }
        }
       
        [HttpGet]
        public async Task<JsonResult> GetStatuses()
        {
            try
            {
                List<Option> viewModel = (await _unitOfWork
                    .Statuses
                    .FindAll<Option>(
                        where: q => !q.IsArchieved,
                        select: q => new Option
                        {
                            Value = q.Id,
                            Label = q.Name
                        })).ToList();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching statuses", ResponseStatus.Fail);
            }
        }

        [HttpPost]
        public async Task<JsonResult> Create([FromBody] AddTicketViewModel viewModel)
        {
            try
            {
                // Problem has not been defined
                if (String.IsNullOrEmpty(viewModel.Problem = viewModel.Problem.Trim()))
                    return ReturnResponse("Please, indicate the ticket's problem.", ResponseStatus.Fail);

                // No status or invalid
                if (String.IsNullOrEmpty(viewModel.Status = viewModel.Status.Trim()) ||
                    !await _unitOfWork.Statuses.isExists(q => q.Id == viewModel.Status))
                    return ReturnResponse("Invalid status provided.", ResponseStatus.Fail);

                // Validating participants (equipments, rooms, personnel)
                if (viewModel.Equipments.Count > 0)
                    foreach (Option op in viewModel.Equipments)
                        if (!await _unitOfWork.Equipments.isExists(q => q.Id == op.Value))
                            return ReturnResponse("One or more Equipments are invalid", ResponseStatus.Fail);

                if (viewModel.Personnel.Count > 0)
                    foreach (Option op in viewModel.Personnel)
                        if (!await _unitOfWork.Personnel.isExists(q => q.Id == op.Value))
                            return ReturnResponse("One or more Personnel are invalid", ResponseStatus.Fail);

                if (viewModel.Rooms.Count > 0)
                    foreach (Option op in viewModel.Rooms)
                        if (!await _unitOfWork.Rooms.isExists(q => q.Id == op.Value))
                            return ReturnResponse("One or more Rooms are invalid", ResponseStatus.Fail);

                if (viewModel.Assignees.Count > 0)
                    foreach (Option op in viewModel.Assignees)
                        if (!await _unitOfWork.TEMSUsers.isExists(q => q.Id == op.Value))
                            return ReturnResponse("One or more Assignees are invalid", ResponseStatus.Fail);

                // Seems valid
                Ticket model = new Ticket
                {
                    Id = Guid.NewGuid().ToString(),
                    StatusId = viewModel.Status,
                    Problem = viewModel.Problem,
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

                if (await _unitOfWork.Tickets.isExists(q => q.Id == model.Id))
                    return ReturnResponse("Success!", ResponseStatus.Success);
                else
                    return ReturnResponse("Fail", ResponseStatus.Fail);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occurred when creating the ticket", ResponseStatus.Fail);
            }
        }
    }

    public static class ExpressionCombiner
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> exp, Expression<Func<T, bool>> newExp)
        {
            // get the visitor
            var visitor = new ParameterUpdateVisitor(newExp.Parameters.First(), exp.Parameters.First());
            // replace the parameter in the expression just created
            newExp = visitor.Visit(newExp) as Expression<Func<T, bool>>;

            // now you can and together the two expressions
            var binExp = Expression.And(exp.Body, newExp.Body);
            // and return a new lambda, that will do what you want. NOTE that the binExp has reference only to te newExp.Parameters[0] (there is only 1) parameter, and no other
            return Expression.Lambda<Func<T, bool>>(binExp, newExp.Parameters);
        }

        class ParameterUpdateVisitor : ExpressionVisitor
        {
            private ParameterExpression _oldParameter;
            private ParameterExpression _newParameter;

            public ParameterUpdateVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
            {
                _oldParameter = oldParameter;
                _newParameter = newParameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (object.ReferenceEquals(node, _oldParameter))
                    return _newParameter;

                return base.VisitParameter(node);
            }
        }
    }
}
