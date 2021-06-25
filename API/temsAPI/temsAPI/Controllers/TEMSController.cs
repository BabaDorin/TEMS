using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
        ILogger<TEMSController> _logger;
        private IMapper mapper;
        private IUnitOfWork unitOfWork;
        private UserManager<TEMSUser> userManager;

        public TEMSController(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            UserManager<TEMSUser> userManager,
            ILogger<TEMSController> logger
            )
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _logger = logger;
        }

        public TEMSController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<TEMSUser> userManager)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
        }

        protected void LogException(Exception ex, object caller = null, string header = null)
        {
            StringBuilder additional = new StringBuilder();
            
            if(caller != null)
                additional.Append(caller.GetType().Name + " - " + new StackTrace().GetFrame(1).GetMethod().Name);

            if (header != null)
                additional.Append(" " + header);

            _logger.Log(LogLevel.Error, ex, additional.ToString() ?? "");
        }

        protected JsonResult ReturnResponse(string message, ResponseStatus status, object additional = null)
        {
            return Json(new { Message = message, Status = status, Additional = additional});
        }
        
        public async Task<List<IArchiveable>> GetArchievedItems(IGenericRepository<IArchiveable> repository) 
        {
            return (await repository.FindAll<IArchiveable>(
                    where: q => q.IsArchieved == true
                )).ToList();
        }
    }
}
