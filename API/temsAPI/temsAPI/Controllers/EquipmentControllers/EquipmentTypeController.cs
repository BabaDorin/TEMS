using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IList<EquipmentType>> Get()
        {
            return await _unitOfWork.EquipmentTypes.FindAll<EquipmentType>(q => q.IsArchieved == false);
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
                        include: q => q.Include(q => q.Children).Include(q => q.Parent),
                        select: q => new ViewEquipmentTypeSimplifiedViewModel
                        {
                            Id = q.Id,
                            Name = q.Name,
                            Children = String.Join(", ", q.Children.Select(q => q.Name)),
                            Parent = (q.ParentId != null)
                                ? q.Parent.Name
                                : null
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

            EquipmentType equipmentType = (await _unitOfWork.EquipmentTypes.Find<EquipmentType>(q => q.Id == typeId,
                includes: new List<string>() { nameof(equipmentType.Properties) }
                )).FirstOrDefault();

            foreach (var item in equipmentType.Properties)
            {
                item.DataType = new DataType();
                item.DataType.Name = _unitOfWork.DataTypes.Find<DataType>(q => q.Id == item.DataTypeID)
                    .Result.FirstOrDefault().Name;
            }

            ViewEquipmentTypeViewModel viewModel = new ViewEquipmentTypeViewModel
            {
                Id = equipmentType.Id,
                Name = equipmentType.Name,
                Properties = _mapper.Map<List<PropertyViewModel>>(equipmentType.Properties)
            };

            return Json(viewModel);
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Insert([FromBody] AddEquipmentTypeViewModel viewModel)
        {
            // Invalid name
            if (String.IsNullOrEmpty((viewModel.Name = viewModel.Name.Trim())))
                return ReturnResponse($"{viewModel.Name} is not a valid type name", ResponseStatus.Fail);

            // Check if this model has already been inserted
            if (await _unitOfWork.EquipmentTypes.isExists(q => q.Name.ToLower() == viewModel.Name.ToLower()))
                return ReturnResponse($"{viewModel.Name} already exists", ResponseStatus.Fail);

            // Invalid parents
            if (viewModel.Parents != null)
                foreach (Option parent in viewModel.Parents)
                    if (!await _unitOfWork.EquipmentTypes.isExists(q => q.Id == parent.Value))
                        return ReturnResponse($"Parent {parent.Label} not found.", ResponseStatus.Fail);


            // Invalid properties
            if (viewModel.Properties != null)
                foreach (Option property in viewModel.Properties)
                    if (!await _unitOfWork.Properties.isExists(q => q.Id == property.Value))
                        return ReturnResponse($"Property {property.Label} not found.", ResponseStatus.Fail);


            // If we got so far, it might be valid
            EquipmentType equipmentType = new EquipmentType
            {
                Id = Guid.NewGuid().ToString(),
                Name = viewModel.Name,
            };

            if (viewModel.Properties != null)
                foreach (Option prop in viewModel.Properties)
                    equipmentType.Properties.Add((await _unitOfWork.Properties.Find<Property>(q => q.Id == prop.Value))
                            .FirstOrDefault());

            await _unitOfWork.EquipmentTypes.Create(equipmentType);
            await _unitOfWork.Save();

            if (!await _unitOfWork.EquipmentTypes.isExists(q => q.Id == equipmentType.Id))
                return ReturnResponse($"Equipment type has not been saved", ResponseStatus.Fail);

            if (viewModel.Parents != null)
                foreach (Option parent in viewModel.Parents)
                {
                    var parentType = (await _unitOfWork.EquipmentTypes
                        .Find<EquipmentType>(q => q.Id == parent.Value))
                        .FirstOrDefault();

                    if(parentType == null)
                        return ReturnResponse("One or more parents are invalid", ResponseStatus.Fail);

                    parentType.Children.Add(equipmentType);
                    await _unitOfWork.Save();
                }

            return ReturnResponse($"Success", ResponseStatus.Success);
        }
    }
}
