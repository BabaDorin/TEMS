using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        [HttpGet]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> Get()
        {
            try
            {
                List<Option> viewModel = new List<Option>();
                viewModel = (await _unitOfWork.EquipmentTypes
                    .FindAll<Option>(
                        where: q => !q.IsArchieved,
                        select: q => new Option
                        {
                            Value = q.Id,
                            Label = q.Name,
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
                        include: q => q.Include(q => q.Children),
                        select: q => new ViewEquipmentTypeSimplifiedViewModel
                        {
                            Id = q.Id,
                            Name = q.Name,
                            Children = String.Join(", ", q.Children.Select(q => q.Name))
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
                    .Include(q => q.Parents)
                    .Include(q => q.Properties)
                )).FirstOrDefault();

            foreach (var item in equipmentType.Properties)
            {
                item.DataType = new DataType();
                item.DataType.Name = _unitOfWork.DataTypes
                    .Find<DataType>(
                        q => q.Id == item.DataTypeID
                     ).Result.FirstOrDefault().Name;
            }

            var test = (await _unitOfWork.EquipmentTypes
                .FindAll<EquipmentType>()).ToList();

            ViewEquipmentTypeViewModel viewModel = new ViewEquipmentTypeViewModel
            {
                Id = equipmentType.Id,
                Name = equipmentType.Name,
                Properties = _mapper.Map<List<PropertyViewModel>>(equipmentType.Properties),
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
                        .Include(q => q.Properties)
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
                    return "The type which is being updated does exist";

            // Check if this model has already been inserted (And it's not the update case)
            if(viewModel.Id == null)
                if (await _unitOfWork.EquipmentTypes.isExists(q => q.Name.ToLower() == viewModel.Name.ToLower()))
                    return $"{viewModel.Name} already exists";

            // Invalid parents
            if (viewModel.Parents != null)
                foreach (Option parent in viewModel.Parents)
                    if (!await _unitOfWork.EquipmentTypes.isExists(q => q.Id == parent.Value))
                        return $"Parent {parent.Label} not found.";


            // Invalid properties
            if (viewModel.Properties != null)
                foreach (Option property in viewModel.Properties)
                    if (!await _unitOfWork.Properties.isExists(q => q.Id == property.Value))
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
                    model.Properties.Add((await _unitOfWork.Properties.Find<Property>(q => q.Id == prop.Value))
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
