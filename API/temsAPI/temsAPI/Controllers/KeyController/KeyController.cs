using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.KeyEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.ViewModels;
using temsAPI.ViewModels.Key;

namespace temsAPI.Controllers.KeyController
{
    public class KeyController : TEMSController
    {
        public KeyController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<TEMSUser> userManager) : base(mapper, unitOfWork, userManager)
        {
        }

        [HttpGet]
        public async Task<JsonResult> Get()
        {
            try
            {
                List<ViewKeySimplifiedViewModel> viewModel = (await _unitOfWork.Keys
                    .FindAll<ViewKeySimplifiedViewModel>(
                        where: q => !q.IsArchieved,
                        include: q => q.Include(qu => qu.KeyAllocations
                            .Where(q => q.DateReturned == null).OrderByDescending(q => q.DateAllocated))
                        .ThenInclude(q => q.Personnel),
                         select: q => new ViewKeySimplifiedViewModel
                         {
                             Id = q.Id,
                             Identifier = q.Identifier,
                             Description = q.Description,
                             NumberOfCopies = q.Copies,
                             AllocatedTo =
                                (q.KeyAllocations.Count > 0)
                                ? new Option
                                {
                                    Value = q.KeyAllocations.First().PersonnelID,
                                    Label = q.KeyAllocations.First().Personnel.Name,
                                }
                                : new Option
                                {
                                    Value = "--",
                                    Label = "--"
                                },
                             TimePassed = (q.KeyAllocations.Count > 0) 
                                ? (DateTime.Now - q.KeyAllocations.First().DateAllocated).TotalMinutes + " minutes ago"
                                : "--"
                         }
                         )).ToList();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching keys", ResponseStatus.Fail);
            }
        }

        [HttpPost]
        public async Task<JsonResult> Create([FromBody] AddKeyViewModel viewModel)
        {
            try
            {
                // No identifier
                if (String.IsNullOrEmpty(viewModel.Identifier = viewModel.Identifier.Trim()))
                    return ReturnResponse("Please, provide a valid key identifier", ResponseStatus.Fail);

                // Invalid Room
                if (viewModel.RoomId != null)
                    if (!await _unitOfWork.Rooms.isExists(q => q.Id == viewModel.RoomId))
                        ReturnResponse("Invalid room provided", ResponseStatus.Fail);

                Key model = _mapper.Map<Key>(viewModel);
                model.RoomId = viewModel.RoomId;
                model.Id = Guid.NewGuid().ToString();

                await _unitOfWork.Keys.Create(model);
                await _unitOfWork.Save();

                if (!await _unitOfWork.Keys.isExists(q => q.Id == model.Id))
                    return ReturnResponse("Fail", ResponseStatus.Fail);
                else
                    return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when creating the key record.", ResponseStatus.Fail);
            }
        }
    }
}
