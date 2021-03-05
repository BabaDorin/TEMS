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

namespace temsAPI.Controllers
{
    public class EquipmentTypeController : TEMSController
    {
        public EquipmentTypeController(IUnitOfWork unitOfWork, UserManager<TEMSUser> userManager)
            : base(unitOfWork, userManager)
        {

        }

        [HttpGet]
        public async Task<IList<EquipmentType>> Get()
        {
            return await _unitOfWork.EquipmentTypes.FindAll(q => q.IsArchieved == false);
        }

        public IActionResult Index()
        {
            return View();
        }

        //[ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<JsonResult> Insert([FromBody]AddEquipmentTypeViewModel viewModel)
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
                    if(!await _unitOfWork.EquipmentTypes.isExists(q => q.ID == parent.Value))
                        return ReturnResponse($"Parent {parent.Label} not found.", Status.Fail);


            // Invalid properties
            if (viewModel.Properties != null)
                foreach (Option property in viewModel.Properties)
                    if(!await _unitOfWork.Properties.isExists(q => q.ID == property.Value))
                        return ReturnResponse($"Property {property.Label} not found.", Status.Fail);


            // If we got so far, it might be valid
            EquipmentType equipmentType = new EquipmentType
            {
                ID = Guid.NewGuid().ToString(),
                Type = viewModel.Name,
            };

            if (viewModel.Properties != null)
                foreach (Option prop in viewModel.Properties)
                    equipmentType.PropertyEquipmentTypeAssociations.Add(new PropertyEquipmentTypeAssociation
                    {
                        ID = Guid.NewGuid().ToString(),
                        PropertyID = prop.Value,
                        TypeID = equipmentType.ID
                    });

            if (viewModel.Properties != null)
                if (viewModel.Parents != null)
                    foreach (Option parent in viewModel.Parents)
                    {
                        equipmentType.EquipmentTypeKinships.Add(new EquipmentTypeKinship
                        {
                            ParentEquipmentTypeId = parent.Value,
                            ChildEquipmentTypeId = equipmentType.ID
                        });
                    }

            await _unitOfWork.EquipmentTypes.Create(equipmentType);
            await _unitOfWork.Save();

            if(await _unitOfWork.EquipmentTypes.isExists(q => q.ID == equipmentType.ID))
                return ReturnResponse($"Success", Status.Succes);
            else
                return ReturnResponse($"Fail", Status.Fail);
        }
    }
}
