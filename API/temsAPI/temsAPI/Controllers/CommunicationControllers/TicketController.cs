using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
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

        [HttpGet("/ticket/{identityType}/{identityId}/{includingClosed}/{onlyClosed}")]
        public async Task<JsonResult> Get(
            string identityType,
            string identityId,
            bool includingClosed,
            bool onlyClosed)
        {
            try
            {
                // Invalid IdentityType
                if ((new List<string> { "equipment", "room", "personnel" }).IndexOf(identityType) == -1)
                    return ReturnResponse("Invalid identity type.", Status.Fail);

                // No identityId Provided
                if (String.IsNullOrEmpty(identityId.Trim()))
                    return ReturnResponse($"You have to provide a valid {identityType} Id", Status.Fail);

                // Checking if identityId is valid and at the same time we build the expression

                // false false
                Expression<Func<Ticket, bool>> ticketExpression = qu => qu.DateClosed == null;

                // true false
                if (includingClosed)
                    ticketExpression = null;

                if (onlyClosed)
                    ticketExpression = qu => qu.DateClosed.HasValue;

                // The tickets that are going to be extracted
                List<Ticket> tickets = new List<Ticket>();

                switch (identityType)
                {
                    case "equipment":
                        if (!await _unitOfWork.Equipments.isExists(q => q.Id == identityId))
                            return ReturnResponse($"There is no {identityType} having the specified Id", Status.Fail);

                        tickets = (await _unitOfWork.Equipments.Find<Equipment>(
                            where: q => q.Id == identityId,
                            include: q => q
                                .Include(q => q.Tickets).ThenInclude(q => q.Status)
                                .Include(q => q.Tickets).ThenInclude(q => q.Label)))
                            .FirstOrDefault()
                            .Tickets.ToList();

                        break;

                    case "room":
                        if (!await _unitOfWork.Rooms.isExists(q => q.Id == identityId))
                            return ReturnResponse($"There is no {identityType} having the specified Id", Status.Fail);

                        tickets = (await _unitOfWork.Rooms.Find<Room>(
                             where: q => q.Id == identityId,
                             include: q => q.Include(q => q.Tickets).ThenInclude(q => q.Equipments)))
                             .FirstOrDefault()
                             .Tickets.ToList();
                        break;

                    case "personnel":
                        if (!await _unitOfWork.Personnel.isExists(q => q.Id == identityId))
                            return ReturnResponse($"There is no {identityType} having the specified Id", Status.Fail);

                        tickets = (await _unitOfWork.Personnel.Find<Personnel>(
                             where: q => q.Id == identityId,
                             include: q => q.Include(q => q.Tickets).ThenInclude(q => q.Equipments)))
                             .FirstOrDefault()
                             .Tickets.ToList();
                        break;
                }

                List<ViewTicketSimplifiedViewModel> viewModel =
                           tickets.AsQueryable().Where(ticketExpression)
                           .ToList()
                           .Select(q => new ViewTicketSimplifiedViewModel
                           {
                               Id = q.Id,
                               DateClosed = (DateTime)q.DateClosed,
                               DateCreated = q.DateCreated,
                               Description = q.Description,
                               Problem = q.Problem,
                               Status = q.Status.Name,
                               Label = new Option { 
                                   Label = q.Label.Name, 
                                   Additional = q.Label.ColorHex
                               },
                               Equipments = q.Equipments
                                               .Select(q => new Option
                                               {
                                                   Label = q.TemsIdOrSerialNumber,
                                                   Value = q.Id
                                               }).ToList(),
                               Personnel = q.Personnel
                                               .Select(q => new Option
                                               {
                                                   Label = q.Name,
                                                   Value = q.Id
                                               }).ToList(),
                               Rooms = q.Rooms
                                           .Select(q => new Option
                                           {
                                               Label = q.Identifier,
                                               Value = q.Id,
                                           }).ToList()
                           }).ToList();

                return (Json(viewModel));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching issues", Status.Fail);
            }
        }
    }
}
