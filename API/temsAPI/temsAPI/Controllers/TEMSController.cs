using AutoMapper;
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
    public enum ResponseStatus
    {
        Success = 1,
        Fail = 0
    }

    public class TEMSController : Controller
    {
        protected static int maxConcurrentUploads = 2;
        protected static int concurrentUploads = 0;

        protected readonly IUnitOfWork _unitOfWork;
        protected readonly UserManager<TEMSUser> _userManager;
        protected IMapper _mapper;

        public TEMSController(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            UserManager<TEMSUser> userManager
            )
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        protected JsonResult ReturnResponse(string message, ResponseStatus status)
        {
            return Json(new { Message = message, Status = status });
        }
    }
}
