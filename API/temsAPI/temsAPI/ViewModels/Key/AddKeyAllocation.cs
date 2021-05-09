using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;

namespace temsAPI.ViewModels.Key
{
    public class AddKeyAllocation
    {
        public List<string> KeyIds { get; set; } = new List<string>();
        public string PersonnelId { get; set; }

        public async Task<string> Validate(IUnitOfWork unitOfWork)
        {
            // No keys or personnel provided
            if (KeyIds.Count == 0 || String.IsNullOrEmpty(PersonnelId))
                return "At least one key and one personnel is needed in order to create an allocation";

            // PersonnelId validity
            if (!await unitOfWork.Personnel.isExists(q => q.Id == PersonnelId))
                return "Invalid personnel provided";

            return null;
        }
    }
}
