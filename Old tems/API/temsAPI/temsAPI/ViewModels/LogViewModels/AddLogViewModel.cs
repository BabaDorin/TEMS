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
        public string Description { get; set; }

        public string Validate()
        {
            // Invalid AddresseesType
            List<string> validAddresseesTypes = new List<string> { "equipment", "room", "personnel" };
            if (validAddresseesTypes.IndexOf(AddresseesType) == -1)
                return "Please, provide a valid Addressee Type";

            // No Addressees provided or the provided ones are invalid
            if (Addressees.Count == 0)
                return "Please, provide at least one Addressee";

            return null;
        }
    }
}
