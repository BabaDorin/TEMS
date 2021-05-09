using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;

namespace temsAPI.ViewModels.Log
{
    public class AddLogViewModel
    {
        public List<Option> Addressees { get; set; }
        public string AddresseesType { get; set; }
        public string LogTypeId { get; set; }
        public string Text { get; set; }
        public bool IsImportant { get; set; }

        public string Validate()
        {
            // Invalid AddresseesType
            List<string> validAddresseesTypes = new List<string> { "equipment", "room", "personnel" };
            if (validAddresseesTypes.IndexOf(AddresseesType) == -1)
                return "Please, provide a valid Addressee Type";

            // No Addressees provided or the provided ones are invalid
            if (Addressees.Count == 0)
                return "Please, provide at least one Addressee";

            // No LogTypeId provided or the provided one is invalid
            if (String.IsNullOrEmpty(LogTypeId))
                return "Please, Provide a log type for the log";

            return null;
        }
    }
}
