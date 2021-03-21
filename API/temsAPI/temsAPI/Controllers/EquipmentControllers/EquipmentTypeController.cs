using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

            EquipmentTypeViewModel viewModel = new EquipmentTypeViewModel
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
                    //equipmentType.PropertyEquipmentTypeAssociations.Add(new PropertyEquipmentTypeAssociation
                    //{
                    //    Id = Guid.NewGuid().ToString(),
                    //    PropertyID = prop.Value,
                    //    TypeID = equipmentType.Id
                    //});
                    equipmentType.Properties.Add((await _unitOfWork.Properties.Find<Property>(q => q.Id == prop.Value))
                            .FirstOrDefault());

            if (viewModel.Parents != null)
                foreach (Option parent in viewModel.Parents)
                {
                    equipmentType.EquipmentTypeKinships.Add(new EquipmentTypeKinship
                    {
                        Id = Guid.NewGuid().ToString(),
                        ParentEquipmentTypeId = parent.Value,
                        ChildEquipmentTypeId = equipmentType.Id
                    });
                }

            await _unitOfWork.EquipmentTypes.Create(equipmentType);
            await _unitOfWork.Save();

            if (await _unitOfWork.EquipmentTypes.isExists(q => q.Id == equipmentType.Id))
                return ReturnResponse($"Success", ResponseStatus.Success);
            else
                return ReturnResponse($"Fail", ResponseStatus.Fail);
        }
    }
}
