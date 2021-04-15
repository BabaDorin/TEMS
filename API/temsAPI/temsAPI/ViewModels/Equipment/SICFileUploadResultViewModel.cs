using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Controllers;

namespace temsAPI.ViewModels.Equipment
{
    public class SICFileUploadResultViewModel
    {
        public string FileName { get; set; }
        public ResponseStatus Status { get; set; }
        public string Message { get; set; }
        public int EllapsedMiliseconds { get; set; }
    }
}
