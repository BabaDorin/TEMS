﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.Email
{
    public class SendEmailViewModel
    {
        public string From { get; set; }
        public List<string> Addressees { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
    }
}