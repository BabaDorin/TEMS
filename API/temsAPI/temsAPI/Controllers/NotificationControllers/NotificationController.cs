using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.Services.Notification;

namespace temsAPI.Controllers.NotificationControllers
{
    public class NotificationController : TEMSController
    {
        private NotificationService _notificationService;
        private NotificationManager _notificationManager;

        public NotificationController(
            IMapper mapper, 
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            NotificationService notificationService,
            NotificationManager notificationManager) : base(mapper, unitOfWork, userManager)
        {
            _notificationService = notificationService;
            _notificationManager = notificationManager;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
