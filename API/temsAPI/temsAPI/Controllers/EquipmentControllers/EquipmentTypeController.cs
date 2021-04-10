using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Controllers;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.System_Files;
using temsAPI.ViewModels;
using temsAPI.ViewModels.EquipmentType;
using temsAPI.ViewModels.Property;

namespace temsAPI.EquipmentControllers
{
    public class EquipmentTypeController : TEMSController
    {
        public EquipmentTypeController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<TEMSUser> userManager)
            : base(mapper, unitOfWork, userManager)
        {

        }

        [HttpGet("equipmenttype/getallautocompleteoptions/{filter?}")]
        public async Task<JsonResult> GetAllAutocompleteOptions(string? filter)
        {
            try
            {
                Expression<Func<EquipmentType, bool>> expression = (filter == null)
                   ? q => !q.IsArchieved
                   : q => !q.IsArchieved && q.Name.Contains(filter);

                List<Option> viewModel = (await _unitOfWork.EquipmentTypes.FindAll<Option>(
                    where: expression,
                    take: 5,
                    select: q => new Option
                    {
                        Value = q.Id,
                        Label = q.Name,
                    })).ToList();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching types", ResponseStatus.Fail);
            }
        }

        [HttpGet]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> GetSimplified()
        {
            try
            {
                List<ViewEquipmentTypeSimplifiedViewModel> viewModel =
                    (await _unitOfWork.EquipmentTypes
                    .FindAll<ViewEquipmentTypeSimplifiedViewModel>(
                        where: q => q.IsArchieved == false,
                        include: q => q.Include(q => q.Children.Where(q => !q.IsArchieved)),
                        select: q => new ViewEquipmentTypeSimplifiedViewModel
                        {
                            Id = q.Id,
                            Name = q.Name,
                            Children = String.Join(", ", q.Children
                            .Where(q => !q.IsArchieved)
                            .Select(q => q.Name))
                        }
                        )).ToList();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching types", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipmenttype/getsimplifiedbyid/{typeId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> GetSimplifiedById(string typeId)
        {
            try
            {
                ViewEquipmentTypeSimplifiedViewModel viewModel =
                    (await _unitOfWork.EquipmentTypes
                    .Find<ViewEquipmentTypeSimplifiedViewModel>(
                        where: q => q.Id == typeId,
                        include: q => q.Include(q => q.Children.Where(q => !q.IsArchieved)),
                        select: q => new ViewEquipmentTypeSimplifiedViewModel
                        {
                            Id = q.Id,
                            Name = q.Name,
                            Children = String.Join(", ", q.Children.Select(q => q.Name))
                        }
                        )).FirstOrDefault();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while fetching type", ResponseStatus.Fail);
            }
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> FullType([FromBody] string typeId)
        {
            if (!await _unitOfWork.EquipmentTypes.isExists(q => q.Id == typeId))
                return ReturnResponse("There is no equipment type associated with the specified typeId", ResponseStatus.Fail);

            EquipmentType equipmentType = (await _unitOfWork.EquipmentTypes
                .Find<EquipmentType>(
                    where: q => q.Id == typeId,
                    include: q => q
                    .Include(q => q.Parents.Where(q => !q.IsArchieved))
                    .Include(q => q.Properties.Where(q => !q.IsArchieved))
                    .ThenInclude(q => q.DataType)))
                .FirstOrDefault();

            var test = (await _unitOfWork.EquipmentTypes
                .FindAll<EquipmentType>()).ToList();

            ViewEquipmentTypeViewModel viewModel = new ViewEquipmentTypeViewModel
            {
                Id = equipmentType.Id,
                Name = equipmentType.Name,
                Properties = equipmentType.Properties
                .Where(q => !q.IsArchieved)
                .Select(q => new ViewPropertyViewModel
                {
                    Description = q.Description,
                    DataType = q.DataType.Name,
                    DisplayName = q.DisplayName,
                    Id = q.Id,
                    Max = q.Max == null ? 0 : (int)q.Max,
                    Min = q.Min == null ? 0 : (int)q.Min,
                    Name = q.Name,
                    Required = q.Required,
                }).ToList(),
                Parents = equipmentType.Parents.Select(q => new Option
                {
                    Value = q.Id,
                    Label = q.Name
                }).ToList()
            };

            return Json(viewModel);
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Add([FromBody] AddEquipmentTypeViewModel viewModel)
        {
            string validationResult = await ValidateEquipmentTypeViewModel(viewModel);
            if (validationResult != null)
                return ReturnResponse(validationResult, ResponseStatus.Fail);

            // If we got so far, it might be valid
            EquipmentType equipmentType = new EquipmentType
            {
                Id = Guid.NewGuid().ToString(),
                Name = viewModel.Name,
            };

            await SetProperties(equipmentType, viewModel);

            await _unitOfWork.EquipmentTypes.Create(equipmentType);
            await _unitOfWork.Save();

            if (!await _unitOfWork.EquipmentTypes.isExists(q => q.Id == equipmentType.Id))
                return ReturnResponse($"Equipment type has not been saved", ResponseStatus.Fail);

            string setParentsResult = await SetParents(equipmentType, viewModel);
            if (setParentsResult != null)
                return ReturnResponse(setParentsResult, ResponseStatus.Fail);

            return ReturnResponse($"Success", ResponseStatus.Success);
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Update([FromBody] AddEquipmentTypeViewModel viewModel)
        {
            try
            {
                string validationResult = await ValidateEquipmentTypeViewModel(viewModel);
                if (validationResult != null)
                    return ReturnResponse(validationResult, ResponseStatus.Fail);

                var equipmentTypeToUpdate = (await _unitOfWork.EquipmentTypes
                    .Find<EquipmentType>(
                        where: q => q.Id == viewModel.Id,
                        include: q => q
                        .Include(q => q.Properties.Where(q => !q.IsArchieved))
                        .Include(q => q.Parents)
                    )).FirstOrDefault();

                if (equipmentTypeToUpdate == null)
                    return ReturnResponse("An error occured, the type has not been found", ResponseStatus.Fail);

                await SetProperties(equipmentTypeToUpdate, viewModel);

                string setParentsResponse = await SetParents(equipmentTypeToUpdate, viewModel);
                if (setParentsResponse != null)
                    return ReturnResponse(setParentsResponse, ResponseStatus.Fail);

                equipmentTypeToUpdate.Name = viewModel.Name;
                await _unitOfWork.Save();
                return ReturnResponse($"Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when saving the type", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipmenttype/remove/{typeId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Remove(string typeId)
        {
            try
            {
                // check if type exists
                var type = (await _unitOfWork.EquipmentTypes
                    .Find<EquipmentType>
                    (
                        where: q => q.Id == typeId
                    )).FirstOrDefault();

                if (type == null)
                    return ReturnResponse("The specified type does not exist", ResponseStatus.Fail);

                type.IsArchieved = true;
                await _unitOfWork.Save();

                return ReturnResponse("Success!", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when removing the type", ResponseStatus.Fail);
            }
        }

        // -----------------------------------------------------------------

        /// <summary>
        /// Validates the view model. Returns null if everything is allright, otherwise - 
        /// returns the issue.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private async Task<string> ValidateEquipmentTypeViewModel(AddEquipmentTypeViewModel viewModel)
        {
            // Invalid name
            if (String.IsNullOrEmpty((viewModel.Name = viewModel.Name.Trim())))
                return $"{viewModel.Name} is not a valid type name";

            // If it's the update case, check if the record exists
            if (viewModel.Id != null)
                if (!await _unitOfWork.EquipmentTypes.isExists(q => q.Id == viewModel.Id))
                    return "The type which is being updated does not exist";

            // Check if this model has already been inserted (And it's not the update case)
            if(viewModel.Id == null)
                if (await _unitOfWork.EquipmentTypes
                    .isExists(q => q.Name.ToLower() == viewModel.Name.ToLower() && !q.IsArchieved))
                    return $"{viewModel.Name} already exists";

            // Invalid parents
            if (viewModel.Parents != null)
                foreach (Option parent in viewModel.Parents)
                    if (!await _unitOfWork.EquipmentTypes.isExists(q => q.Id == parent.Value))
                        return $"Parent {parent.Label} not found.";


            // Invalid properties
            if (viewModel.Properties != null)
                foreach (Option property in viewModel.Properties)
                    if (!await _unitOfWork.Properties.isExists(q => q.Id == property.Value && !q.IsArchieved))
                        return $"Property {property.Label} not found.";

            return null;
        }

        /// <summary>
        /// Sets properties from view model to the model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private async Task SetProperties(EquipmentType model, AddEquipmentTypeViewModel viewModel)
        {
            if (model.Properties.Select(q => q.Id).SequenceEqual(viewModel.Properties.Select(q => q.Value)))
                return;

            var modelProperties = model.Properties.ToList();
            foreach (var item in modelProperties)
            {
                model.Properties.Remove(item);
            }
            await _unitOfWork.Save();

            if (viewModel.Properties != null)
                foreach (Option prop in viewModel.Properties)
                {
                    model.Properties.Add((await _unitOfWork.Properties.Find<Property>(q => q.Id == prop.Value && !q.IsArchieved))
                            .FirstOrDefault());

                    await _unitOfWork.Save();
                }
        }

        /// <summary>
        /// Sets the parents from view model to the model. Returns null if success, otherwise - 
        /// returns the issue.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private async Task<string> SetParents(EquipmentType model, AddEquipmentTypeViewModel viewModel)
        {
            if (viewModel.Parents == null || viewModel.Parents.Count == 0)
                return null;

            if (model.Parents.Select(q => q.Id).SequenceEqual(viewModel.Parents.Select(q => q.Value)))
                return null;

            var modelParents = model.Parents.ToList();
            foreach (var item in modelParents)
            {
                model.Parents.Remove(item);
            }
            await _unitOfWork.Save();

            if (viewModel.Parents != null)
                foreach (Option parent in viewModel.Parents)
                {
                    var parentType = (await _unitOfWork.EquipmentTypes
                        .Find<EquipmentType>(q => q.Id == parent.Value))
                        .FirstOrDefault();

                    if (parentType == null || parentType.Id == model.Id)
                        return "One or more parents are invalid";

                    model.Parents.Add(parentType);
                    //parentType.Children.Add(model);
                    await _unitOfWork.Save();
                }

            return null;
        }
    }
}
