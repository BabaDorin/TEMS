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
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.ViewModels;
using temsAPI.ViewModels.EquipmentType;
using temsAPI.ViewModels.Property;

namespace temsAPI.Controllers
{
    public class EquipmentTypeController : TEMSController
    {
        public EquipmentTypeController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<TEMSUser> userManager)
            : base(mapper, unitOfWork, userManager)
        {

        }

        [HttpGet]
        public async Task<IList<EquipmentType>> Get()
        {
            return await _unitOfWork.EquipmentTypes.FindAll(q => q.IsArchieved == false);
        }

        [HttpPost]
        public async Task<JsonResult> GetFullType([FromBody]string typeId)
        {
            if (!await _unitOfWork.EquipmentTypes.isExists(q => q.Id == typeId))
                return ReturnResponse("There is no equipment type associated with the specified typeId", Status.Fail);

            EquipmentType equipmentType = await _unitOfWork.EquipmentTypes.Find(q => q.Id == typeId,
                includes: new List<string>() { nameof(equipmentType.Properties) }
                );

            foreach (var item in equipmentType.Properties)
                item.DataType = await _unitOfWork.DataTypes.Find(q => q.Id == item.DataTypeID);

            //equipmentType = _unitOfWork.
            EquipmentTypeViewModel viewModel = new EquipmentTypeViewModel
            {
                Id = equipmentType.Id,
                Name = equipmentType.Type,
                Properties = _mapper.Map<List<PropertyViewModel>>(equipmentType.Properties)
            };

            return Json(viewModel);
        }

        public IActionResult Index()
        {
            return View();
        }

        //[ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<JsonResult> Insert([FromBody] AddEquipmentTypeViewModel viewModel)
        {
            // Invalid name
            if (String.IsNullOrEmpty((viewModel.Name = viewModel.Name.Trim())))
                return ReturnResponse($"{viewModel.Name} is not a valid type name", Status.Fail);

            // Check if this model has already been inserted
            if (await _unitOfWork.EquipmentTypes.isExists(q => q.Type.ToLower() == viewModel.Name.ToLower()))
                return ReturnResponse($"{viewModel.Name} already exists", Status.Fail);

            // Invalid parents
            if (viewModel.Parents != null)
                foreach (Option parent in viewModel.Parents)
                    if (!await _unitOfWork.EquipmentTypes.isExists(q => q.Id == parent.Value))
                        return ReturnResponse($"Parent {parent.Label} not found.", Status.Fail);


            // Invalid properties
            if (viewModel.Properties != null)
                foreach (Option property in viewModel.Properties)
                    if (!await _unitOfWork.Properties.isExists(q => q.Id == property.Value))
                        return ReturnResponse($"Property {property.Label} not found.", Status.Fail);


            // If we got so far, it might be valid
            EquipmentType equipmentType = new EquipmentType
            {
                Id = Guid.NewGuid().ToString(),
                Type = viewModel.Name,
            };

            if (viewModel.Properties != null)
                foreach (Option prop in viewModel.Properties)
                    //equipmentType.PropertyEquipmentTypeAssociations.Add(new PropertyEquipmentTypeAssociation
                    //{
                    //    Id = Guid.NewGuid().ToString(),
                    //    PropertyID = prop.Value,
                    //    TypeID = equipmentType.Id
                    //});
                    equipmentType.Properties.Add(await _unitOfWork.Properties.Find(q => q.Id == prop.Value));

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
                return ReturnResponse($"Success", Status.Succes);
            else
                return ReturnResponse($"Fail", Status.Fail);
        }
    }
}
