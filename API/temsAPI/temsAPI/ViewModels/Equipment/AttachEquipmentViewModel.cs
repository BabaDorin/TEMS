using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using temsAPI.Contracts;

namespace temsAPI.ViewModels.Equipment
{
    public class AttachEquipmentViewModel
    {
        public string ParentId { get; set; }
        public List<string> ChildrenIds { get; set; }

        /// <summary>
        /// Validates an instance of AttachEquipmentViewModel. Returns null if everything is Ok,
        /// otherwise returns the error message.
        /// </summary>
        /// <returns></returns>
        public async Task<string> Validate(IUnitOfWork unitOfWork)
        {
            if (ChildrenIds == null || ChildrenIds.Count < 1)
                return "At least one children Id is required";

            ChildrenIds = ChildrenIds.Distinct().ToList();

            if (!await unitOfWork.Equipments.isExists(q => q.Id == ParentId))
                return "Invalid parent ID provided";

            foreach(var child in ChildrenIds)
            {
                if (!await unitOfWork.Equipments
                    .isExists(q => q.Id == child && !q.IsArchieved && q.ParentID != ParentId))
                    return "One or more children equipment are invalid." +
                        " Only un-archieved equipment allowed. Also, make sure you're not re-attaching the same equipment again to it's parent";
            }

            return null;
        }
    }
}
