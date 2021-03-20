using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.ViewModels.IdentityViewModels;

namespace temsAPI.Controllers.IdentityControllers
{
    public class TEMSUserController : TEMSController
    {
        public TEMSUserController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<TEMSUser> userManager) : base(mapper, unitOfWork, userManager)
        {
        }

        [HttpPost]
        public async Task<JsonResult> Register([FromBody] RegisterViewModel viewModel)
        {
            try
            {
                TEMSUser model = _mapper.Map<TEMSUser>(viewModel);
                var result = await _userManager.CreateAsync(model, viewModel.Password);

                // Roles assignation later
                return ReturnResponse("The user has been saved", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse(
                    "An error occured when saving the user, consider trying again", 
                    ResponseStatus.Fail);
            }
        }

        [HttpPost]
        public async Task<IActionResult> LogIn([FromBody] LogInViewModel viewModel)
        {
            throw new NotImplementedException();
        }
    }
}
