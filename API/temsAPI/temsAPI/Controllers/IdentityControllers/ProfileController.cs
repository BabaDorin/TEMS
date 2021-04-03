using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.System_Files;
using temsAPI.ViewModels;
using temsAPI.ViewModels.Profile;

namespace temsAPI.Controllers.IdentityControllers
{
    [Authorize]
    public class ProfileController : TEMSController
    {
        public ProfileController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<TEMSUser> userManager) : base(mapper, unitOfWork, userManager)
        {
        }

        [HttpGet("profile/get/{userId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> Get(string userId)
        {
            try
            {
                var viewModel = (await _unitOfWork.TEMSUsers
                    .Find<ViewProfileViewModel>(
                        where: q => q.Id == userId,
                        include: q => q
                        .Include(q => q.AssignedTickets)
                        .Include(q => q.ClosedTickets)
                        .Include(q => q.CreatedTickets)
                        .Include(q => q.Announcements)
                        .Include(q => q.Personnel).ThenInclude(q => q.KeyAllocations),
                        select: q => new ViewProfileViewModel
                        {
                            Id = q.Id,
                            FullName = q.FullName,
                            Username = q.UserName,
                            Email = q.Email,
                            IsArchieved = q.IsArchieved,
                            //DateRegistered = q.DateRegistered,
                            PhoneNumber = q.PhoneNumber,
                            Personnel = (q.Personnel == null)
                                ? null
                                : new Option
                                {
                                    Value = q.PersonnelId,
                                    Label = q.Personnel.Name,
                                },
                        }
                    )).FirstOrDefault();

                viewModel.Roles = (await _userManager
                    .GetRolesAsync(await _userManager.FindByIdAsync(userId)))
                    .ToList();
                
                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while fetching user data", ResponseStatus.Fail);
            }
        } 
    }
}
