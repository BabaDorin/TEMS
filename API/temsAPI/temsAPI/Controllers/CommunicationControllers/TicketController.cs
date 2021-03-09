using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.ViewModels.Ticket;

namespace temsAPI.Controllers.CommunicationControllers
{
    public class TicketController : TEMSController
    {
        public TicketController(
            IMapper mapper, 
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager): base(mapper, unitOfWork, userManager)
        {
        }

        //[HttpGet("/ticket/{identityType}/{identityId}/{includingClosed}/{onlyClosed}")]
        //public async Task<JsonResult> Get(
        //    string identityType,
        //    string identityId,
        //    bool includingClosed,
        //    bool onlyClosed)
        //{
        //    try
        //    {
        //        // Invalid IdentityType
        //        if ((new List<string> { "equipment", "room", "personnel" }).IndexOf(identityType) == -1)
        //            return ReturnResponse("Invalid identity type.", Status.Fail);

        //        // No identityId Provided
        //        if (String.IsNullOrEmpty(identityId.Trim()))
        //            return ReturnResponse($"You have to provide a valid {identityType} Id", Status.Fail);

        //        // Checking if identityId is valid and at the same time we build the expression
        //        Expression<Func<Ticket, bool>> expression = null;

        //        switch (identityType)
        //        {
        //            case "equipment":
        //                if (!await _unitOfWork.Equipments.isExists(q => q.Id == identityId))
        //                    return ReturnResponse($"There is no {identityType} having the specified Id", Status.Fail);

        //                expression = qu => qu.EquipmentId == identityId && qu.DateClosed == null;
        //                if (includingClosed)
        //                    expression = qu => qu.EquipmentId == identityId;
        //                if (onlyClosed)
        //                    expression = qu => qu.EquipmentId == identityId && qu.DateClosed != null;

        //                break;

        //            case "room":
        //                if (!await _unitOfWork.Rooms.isExists(q => q.Id == identityId))
        //                    return ReturnResponse($"There is no {identityType} having the specified Id", Status.Fail);

        //                expression = qu => qu.RoomID == identityId && qu.DateClosed == null;
        //                if (includingClosed)
        //                    expression = qu => qu.RoomID == identityId;
        //                if (onlyClosed)
        //                    expression = qu => qu.RoomID == identityId && qu.DateClosed != null;
        //                break;

        //            case "personnel":
        //                if (!await _unitOfWork.Personnel.isExists(q => q.Id == identityId))
        //                    return ReturnResponse($"There is no {identityType} having the specified Id", Status.Fail);

        //                expression = qu => qu.PersonnelId == identityId && qu.DateClosed == null;
        //                if (includingClosed)
        //                    expression = qu => qu.PersonnelId == identityId;
        //                if (onlyClosed)
        //                    expression = qu => qu.PersonnelId == identityId && qu.DateClosed != null;
        //                break;
        //        }

        //        var viewModel = await _unitOfWork.Tickets.FindAll<ViewTicketSimplifiedViewModel>(
        //            where: expression,
        //            include: q => q
        //                .Include(q => q.Room)
        //                .Include(q => q.Personnel)
        //                .Include(q => q.Equipment),
        //            select: q => new ViewTicketSimplifiedViewModel { 
        //                Id = q.Id,
        //                DateCreated = q.DateCreated,
        //                DateClosed = (DateTime)q.DateClosed,
        //                Description = q.Description,
        //                Equipments // change the logic of how backend handles tickets... 
        //            });
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex);
        //        return ReturnResponse("An error occured when fetching issues", Status.Fail);
        //    }
        //}
    }
}
