using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Controllers;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.System_Files;
using temsAPI.Validation;
using temsAPI.ViewModels;
using temsAPI.ViewModels.Property;

namespace temsAPI.EquipmentControllers
{
    public class PropertyController : TEMSController
    {
        public PropertyController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<TEMSUser> userManager) 
            : base(mapper, unitOfWork, userManager)
        {

        }

        [HttpGet]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> Get()
        {
            try
            {
                return Json(await _unitOfWork.Properties.FindAll<Property>());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching properties", ResponseStatus.Fail);
            }
        }

        [HttpGet]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> GetSimplified()
        {
            try
            {
                List<ViewPropertySimplifiedViewModel> viewModel =
                    (await _unitOfWork.Properties
                    .FindAll(
                        select: q => new ViewPropertySimplifiedViewModel
                        {
                            Id = q.Id,
                            Description = q.Description,
                            DisplayName = q.DisplayName
                        }
                    )).ToList();
                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching properties", ResponseStatus.Fail);
            }
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Insert([FromBody] AddPropertyViewModel viewModel)
        {
            // name is null
            if(String.IsNullOrEmpty(viewModel.Name.Trim()))
                return ReturnResponse("'Name' is required.", ResponseStatus.Fail);

            // name - no spaces or special chars
            if ((viewModel.Name = viewModel.Name.Trim()).Any(Char.IsWhiteSpace))
                return ReturnResponse("'Name' value can not contain spaces.", ResponseStatus.Fail);

            if(!RegexValidation.OnlyAlphaNumeric.IsMatch(viewModel.Name))
                return ReturnResponse("'Name' value can not contain non-alphanumeric characters," +
                    " Allowed: only a-z, A-Z, 0-9.", ResponseStatus.Fail);

            // displayName is null
            if(string.IsNullOrEmpty(viewModel.DisplayName.Trim()))
                return ReturnResponse("'DisplayName' is required.", ResponseStatus.Fail);

            // dataType should be a valid one
            viewModel.DataType = viewModel.DataType.ToLower().Trim();
            if(!await _unitOfWork.DataTypes.isExists(q => q.Name == viewModel.DataType))
                return ReturnResponse("The datatype that has been provided seems invalid.", ResponseStatus.Fail);

            if (!String.IsNullOrEmpty(viewModel.Description))
                viewModel.Description = viewModel.Description.Trim();

            // Check if the property already exists
            if(await _unitOfWork.Properties
                .isExists(q => q.Name.ToLower() == viewModel.Name.ToLower() ||
                               q.DisplayName.ToLower() == viewModel.DisplayName.ToLower()))
                return ReturnResponse($"This property already exists", ResponseStatus.Fail);

            // If we got so far, it might be valid.
            Property property = new Property()
            {
                Id = Guid.NewGuid().ToString(),
                Name = viewModel.Name,
                DisplayName = viewModel.DisplayName,
                Required = viewModel.Required,
                DataType = (await _unitOfWork.DataTypes.Find<DataType>(q => q.Name.ToLower() == viewModel.DataType))
                    .FirstOrDefault(),
            };

            await _unitOfWork.Properties.Create(property);
            await _unitOfWork.Save();

            if (await _unitOfWork.Properties.isExists(q => q.Id == property.Id))
                return ReturnResponse($"Success", ResponseStatus.Success);
            else
                return ReturnResponse($"Fail", ResponseStatus.Fail);
        }
    }
}
