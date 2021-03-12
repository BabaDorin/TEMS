﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.Key
{
    public class ViewKeySimplifiedViewModel
    {
        public string Id { get; set; }
        public string Identifier { get; set; }
        public int NumberOfCopies { get; set; }
        public Option AllocatedTo { get; set; }
        public TimeSpan TimePassed { get; set; }
        public string Description { get; set; }
    }
}
