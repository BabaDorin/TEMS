using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Controllers
{
    public enum Status
    {
        Succes = 1,
        Fail = 0
    }

    public class TEMSController : Controller
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly UserManager<TEMSUser> _userManager;

        public TEMSController(
            IUnitOfWork unitOfWork,
            UserManager<TEMSUser> userManager
            )
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        protected JsonResult ReturnResponse(string message, Status status)
        {
            return Json(new { Message = message, Statis = status });
        }
    }
}
