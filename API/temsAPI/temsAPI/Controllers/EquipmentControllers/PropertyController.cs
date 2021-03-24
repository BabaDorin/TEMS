using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                List<Option> viewModel = (await _unitOfWork.Properties
                    .FindAll<Option>(
                        select: q => new Option
                        {
                            Value = q.Id,
                            Label = q.DisplayName
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

        [HttpGet("property/getbyid/{propertyId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> GetById(string propertyId)
        {
            try
            {
                Property model = ((await _unitOfWork.Properties
                    .Find<Property>(
                        where: q => q.Id == propertyId,
                        include: q => q.Include(q => q.DataType)
                    )).FirstOrDefault());

                ViewPropertyViewModel viewModel = _mapper.Map<ViewPropertyViewModel>(model);
                viewModel.DataType = model.DataType.Name.ToLower();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching the property", ResponseStatus.Fail);
            }
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Add([FromBody] AddPropertyViewModel viewModel)
        {

            string validationResult = await ValidateAddPropertyViewModel(viewModel);
            if (validationResult != null)
                return ReturnResponse(validationResult, ResponseStatus.Fail);

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

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Update([FromBody] AddPropertyViewModel viewModel)
        {
            try
            {
                string validationResult = await ValidateAddPropertyViewModel(viewModel);
                if (validationResult != null)
                    return ReturnResponse(validationResult, ResponseStatus.Fail);

                var propertyToUpdate = (await _unitOfWork.Properties
                    .Find<Property>(
                        where: q => q.Id == viewModel.Id
                    )).FirstOrDefault();

                propertyToUpdate.Name = viewModel.Name;
                propertyToUpdate.DisplayName = viewModel.DisplayName;
                propertyToUpdate.Description = viewModel.Description;
                propertyToUpdate.DataType = (await _unitOfWork.DataTypes.
                    Find<DataType>(q => q.Name.ToLower() == viewModel.DataType.ToLower()))
                    .FirstOrDefault();
                propertyToUpdate.Required = viewModel.Required;

                await _unitOfWork.Save();

                return ReturnResponse("Success!", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when saving the property", ResponseStatus.Fail);
            }
        }

        // -----------------------------------------------------------------

        /// <summary>
        /// Validates an AddPropertyViewModel instance and trims it's properties' values.
        /// It returns null if everything is ok, otherwise - returns the error message.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private async Task<string> ValidateAddPropertyViewModel(AddPropertyViewModel viewModel)
        {
            viewModel.Name = viewModel.Name?.Trim();

            // name is null
            if (String.IsNullOrEmpty(viewModel.Name))
                return "'Name' is required.";

            // name - no spaces or special chars
            if ((viewModel.Name).Any(Char.IsWhiteSpace))
                return "'Name' value can not contain spaces.";

            if (!RegexValidation.OnlyAlphaNumeric.IsMatch(viewModel.Name))
                return "'Name' value can not contain non-alphanumeric characters," +
                    " Allowed: only a-z, A-Z, 0-9.";

            // displayName is null
            if (string.IsNullOrEmpty(viewModel.DisplayName.Trim()))
                return "'DisplayName' is required.";

            // dataType should be a valid one
            viewModel.DataType = viewModel.DataType?.ToLower().Trim();
            if (!await _unitOfWork.DataTypes.isExists(q => q.Name == viewModel.DataType))
                return "The datatype that has been provided seems invalid.";

            viewModel.Description = viewModel.Description?.Trim();

            // If it's the update case
            if (viewModel.Id != null)
                if (!await _unitOfWork.Properties.isExists(q => q.Id == viewModel.Id))
                    return "The property that is being updated does not exist.";

            // Check if the property already exists (and it's not the update case)
            if(viewModel.Id == null)
                if (await _unitOfWork.Properties
                    .isExists(q => q.Name.ToLower() == viewModel.Name.ToLower() ||
                                   q.DisplayName.ToLower() == viewModel.DisplayName.ToLower()))
                        return "This property already exists";

            return null;
        }
    }
}
