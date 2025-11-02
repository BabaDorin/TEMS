using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.Library
{
    public class AddLibraryItemViewModel
    {
        public string ActualName { get; set; }
        public string DisplayName { get; set; }

        public string Description { get; set; }
    }
}
