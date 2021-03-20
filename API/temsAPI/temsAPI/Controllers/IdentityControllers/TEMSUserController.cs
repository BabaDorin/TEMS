using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
        public async Task<JsonResult> AddUser([FromBody] AddUserViewModel viewModel)
        {
            try
            {
                // Username and password are mandatory
                viewModel.Username = viewModel.Username?.Trim();
                if(String.IsNullOrEmpty(viewModel.Username) || String.IsNullOrEmpty(viewModel.Password))
                    return ReturnResponse("Invalid username or password provided", ResponseStatus.Fail);

                TEMSUser model = new TEMSUser()
                {
                    UserName = viewModel.Username,
                    PhoneNumber = viewModel.PhoneNumber,
                    Email = viewModel.Email,
                };
                var result = await _userManager.CreateAsync(model, viewModel.Password);
                
                if(result.Errors.Count() == 0)
                    return ReturnResponse("The user has been saved", ResponseStatus.Success);

                StringBuilder stringBuilder = new StringBuilder();
                foreach (var error in result.Errors)
                {
                    stringBuilder.Append(error.Description + "\n");
                }

                return ReturnResponse(stringBuilder.ToString(), ResponseStatus.Fail);
                // Roles assignation later
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
