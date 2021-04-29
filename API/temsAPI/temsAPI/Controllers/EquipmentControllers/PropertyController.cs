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
using temsAPI.Helpers;
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
                        where: q => !q.IsArchieved,
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
                        where: q => !q.IsArchieved,
                        select: q => new ViewPropertySimplifiedViewModel
                        {
                            Id = q.Id,
                            Description = q.Description,
                            DisplayName = q.DisplayName,
                            Editable = (bool)q.EditablePropertyInfo
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

        [HttpGet("property/getsimplifiedbyid/{propertyId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> GetSimplifiedById(string propertyId)
        {
            try
            {
                ViewPropertySimplifiedViewModel viewModel =
                    (await _unitOfWork.Properties
                    .Find(
                        where: q => q.Id == propertyId,
                        select: q => new ViewPropertySimplifiedViewModel
                        {
                            Id = q.Id,
                            Description = q.Description,
                            DisplayName = q.DisplayName,
                            Editable = (bool)q.EditablePropertyInfo
                        }
                    )).FirstOrDefault();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching the property", ResponseStatus.Fail);
            }
        }

        [HttpGet("property/getbyid/{propertyId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> GetById(string propertyId)
        {
            try
            {
                ViewPropertyViewModel viewModel = ((await _unitOfWork.Properties
                    .Find<ViewPropertyViewModel>(
                        where: q => q.Id == propertyId,
                        include: q => q.Include(q => q.DataType),
                        select: q => new ViewPropertyViewModel
                        {
                            Id = q.Id,
                            DataType = q.DataType.Name.ToLower(),
                            Description = q.Description,
                            DisplayName = q.DisplayName,
                            Max = (int)q.Max,
                            Min = (int)q.Min,
                            Name = q.Name,
                            Required = q.Required,
                        }
                    )).FirstOrDefault());

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

            string validationResult = await viewModel.Validate(_unitOfWork);
            if (validationResult != null)
                return ReturnResponse(validationResult, ResponseStatus.Fail);

            // If we got so far, it might be valid.
            Property property = new Property()
            {
                Id = Guid.NewGuid().ToString(),
                Name = viewModel.Name,
                DisplayName = viewModel.DisplayName,
                Description = viewModel.Description,
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

        [HttpGet("property/getpropertiesoftype/{typeId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetPropertiesOfType(string typeId)
        {
            try
            {
                List<Option> viewModel = (await _unitOfWork.EquipmentTypes
                    .Find<List<Option>>(
                        include: q => q.Include(q => q.Properties),
                        where: q => q.Id == typeId,
                        select: q => q.Properties.Select(q => new Option
                        {
                            Value = q.Id,
                            Label = q.DisplayName,
                            Additional = q.Name
                        }).ToList()
                    )).FirstOrDefault();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while fetching properties.", ResponseStatus.Fail);
            }
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Update([FromBody] AddPropertyViewModel viewModel)
        {
            try
            {
                string validationResult = await viewModel.Validate(_unitOfWork);
                if (validationResult != null)
                    return ReturnResponse(validationResult, ResponseStatus.Fail);

                var propertyToUpdate = (await _unitOfWork.Properties
                    .Find<Property>(
                        where: q => q.Id == viewModel.Id
                    )).FirstOrDefault();

                if ((bool)!propertyToUpdate.EditablePropertyInfo)
                    return ReturnResponse("This property can not be edited.", ResponseStatus.Fail);

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

        [HttpGet("property/archieve/{propertyId}/{archivationStatus?}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Archieve(string propertyId, bool archivationStatus = true)
        {
            try
            {
                var archievingResult = await (new ArchieveHelper(_userManager, _unitOfWork))
                     .SetPropertyArchivationStatus(propertyId, archivationStatus);

                if (archievingResult != null)
                    return ReturnResponse(archievingResult, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while changing the archivation status.", ResponseStatus.Fail);
            }
        }
    }
}
