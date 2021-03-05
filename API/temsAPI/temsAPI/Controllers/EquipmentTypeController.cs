using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.UserEntities;
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

        [HttpPost]
        public JsonResult Insert([FromBody]AddEquipmentTypeViewModel viewModel)
        {
            Debug.WriteLine(viewModel);
            return Json(new { Name = viewModel.Name });
        }
    }
}
