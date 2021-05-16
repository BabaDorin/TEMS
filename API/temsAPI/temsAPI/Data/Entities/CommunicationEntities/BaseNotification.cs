﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;

namespace temsAPI.Data.Entities.CommunicationEntities
{
    public class BaseNotification : INotification
    {
        public DateTime DateCreated { get; set; }
        public string Message { get; set; }
        public bool SendEmail { get; set; }
        public bool SendSMS { get; set; }
        public bool SendPush { get; set; }
        public bool SendBrowser { get; set; }
    }
}
