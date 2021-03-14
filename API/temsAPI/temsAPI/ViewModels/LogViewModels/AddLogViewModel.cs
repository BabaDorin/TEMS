using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.Log
{
    public class AddLogViewModel
    {
        public List<Option> Addressees { get; set; }
        public string AddresseesType { get; set; }
        public string LogTypeId { get; set; }
        public string Text { get; set; }
        public bool IsImportant { get; set; }

    }
}
