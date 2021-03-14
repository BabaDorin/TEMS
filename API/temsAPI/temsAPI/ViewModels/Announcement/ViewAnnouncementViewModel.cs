﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.Announcement
{
    public class ViewAnnouncementViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public Option CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
    }
}