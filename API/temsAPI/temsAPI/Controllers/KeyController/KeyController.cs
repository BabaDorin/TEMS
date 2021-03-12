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
                                    Value = "none",
                                    Label = "None"
                                },
                             TimePassed = (q.KeyAllocations.Count > 0) 
                                ? DateTime.Now - q.KeyAllocations.First().DateAllocated
                                : TimeSpan.FromSeconds(0)
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
    }
}
