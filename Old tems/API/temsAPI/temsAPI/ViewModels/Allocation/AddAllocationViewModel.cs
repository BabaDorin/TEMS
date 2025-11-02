using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;

namespace temsAPI.ViewModels.Allocation
{
    public class AddAllocationViewModel
    {
        public List<Option> Equipments { get; set; } = new List<Option>();
        public string AllocateToType { get; set; }
        public string AllocateToId { get; set; }

        public async Task<string> Validate(IUnitOfWork unitOfWork)
        {
            // Invalid equipments provided
            foreach (Option equipment in Equipments)
                if (!await unitOfWork.Equipments.isExists(q => q.Id == equipment.Value))
                    return "One or more equipments are invalid.";

            // No allocation type provided
            if ((new List<string> { "personnel", "room" }).IndexOf(AllocateToType) == -1)
                return "Invalid type provided";

            // No allocation id provided or the provided one is invalid
            if (String.IsNullOrEmpty(AllocateToType))
                return "Please, provide a valid allocation object type";

            return null;
        }
    }
}
