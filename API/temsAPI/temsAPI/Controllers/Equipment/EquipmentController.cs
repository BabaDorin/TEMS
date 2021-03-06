using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.ViewModels.Equipment;

namespace temsAPI.Controllers.Equipment
{
    public class EquipmentController : TEMSController
    {

        public EquipmentController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<TEMSUser> userManager)
           : base(mapper, unitOfWork, userManager)
        {

        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Create([FromBody] AddEquipmentViewModel viewModel)
        {
            // one or both of TEMSID and SerialNumber properties should be given values
            if ((viewModel.Temsid == null || 
                String.IsNullOrEmpty(viewModel.Temsid = viewModel.Temsid.Trim())) &&
                (viewModel.SerialNumber == null || 
                String.IsNullOrEmpty(viewModel.SerialNumber = viewModel.SerialNumber.Trim())))
                return ReturnResponse("Please, provide information for TemsID and / or SerialNumber", Status.Fail);

            // Equipment is already created
            if (!String.IsNullOrEmpty(viewModel.Temsid) &&
                await _unitOfWork.Equipments.isExists(q => q.TEMSID == viewModel.Temsid) ||
                !String.IsNullOrEmpty(viewModel.SerialNumber) &&
                await _unitOfWork.Equipments.isExists(q => q.SerialNumber == viewModel.SerialNumber))
                return ReturnResponse("This equipment already exists.", Status.Fail);



            // Value for purchase date wasn't provided
            if (viewModel.PurchaseDate == new DateTime())
                viewModel.PurchaseDate = DateTime.Now;

            // Invalid price data
            if (viewModel.Price < 0 ||
                (new List<string> { "lei", "eur", "usd" }).IndexOf(viewModel.Currency) == -1)
                return ReturnResponse("Invalid price data provided.", Status.Fail);

            // Invalid definition provided
            if(!await _unitOfWork.EquipmentDefinitions.isExists(q => q.Id == viewModel.EquipmentDefinitionID))
                return ReturnResponse("An equipment definition having the specified id has not been found.", Status.Fail);

            // If we got so far, it might be valid
            Data.Entities.EquipmentEntities.Equipment model =
                _mapper.Map<Data.Entities.EquipmentEntities.Equipment>(viewModel);

            model.Id = Guid.NewGuid().ToString();

            await _unitOfWork.Equipments.Create(model);
            await _unitOfWork.Save();

            if(!await _unitOfWork.Equipments.isExists(q => q.Id == model.Id))
                return ReturnResponse("Fail", Status.Fail);

            return ReturnResponse("Success", Status.Success);
        }
    }
}
