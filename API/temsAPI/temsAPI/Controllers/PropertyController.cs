using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.ViewModels;

namespace temsAPI.Controllers
{
    public class PropertyController : TEMSController
    {
        public PropertyController(IUnitOfWork unitOfWork, UserManager<TEMSUser> userManager) 
            : base(unitOfWork, userManager)
        {

        }

        [HttpGet]
        public async Task<IEnumerable<Property>> Get()
        {
            return await _unitOfWork.Properties.FindAll();
        }
    }
}
