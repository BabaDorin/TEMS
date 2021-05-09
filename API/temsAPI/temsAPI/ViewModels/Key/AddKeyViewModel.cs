using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;

namespace temsAPI.ViewModels.Key
{
    public class AddKeyViewModel
    {
        public string Identifier { get; set; }
        public int NumberOfCopies { get; set; }
        public string RoomId { get; set; }
        public string Description { get; set; }

        public async Task<string> Validate(IUnitOfWork unitOfWork)
        {
            Identifier = Identifier?.Trim();
            if (String.IsNullOrEmpty(Identifier))
                return "Please, provide a valid key identifier";

            if (RoomId != null)
                if (!await unitOfWork.Rooms.isExists(q => q.Id == RoomId))
                    return "Invalid room provided";

            if (NumberOfCopies > 5)
                return "Maximum 5 copies at a time.";

            return null;
        }
    }
}
