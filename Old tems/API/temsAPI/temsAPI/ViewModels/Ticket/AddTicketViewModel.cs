using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;

namespace temsAPI.ViewModels.Ticket
{
    public class AddTicketViewModel
    {
        public string Problem { get; set; }
        public string ProblemDescription { get; set; }
        public string Status { get; set; }
        public List<Option> Rooms { get; set; }
        public List<Option> Personnel { get; set; }
        public List<Option> Equipments { get; set; }
        public List<Option> Assignees { get; set; }
        

        public async Task<string> Validate(IUnitOfWork unitOfWork)
        {
            // Problem has not been defined
            Problem = Problem?.Trim();
            if (String.IsNullOrEmpty(Problem))
                return "Please, indicate the ticket's problem.";

            // No status or invalid
            Status = Status?.Trim();
            if (String.IsNullOrEmpty(Status) ||
                !await unitOfWork.Statuses.isExists(q => q.Id == Status))
                return "Invalid status provided.";

            // Validating participants (equipments, rooms, personnel)
            if (Equipments.Count > 0)
                foreach (Option op in Equipments)
                    if (!await unitOfWork.Equipments.isExists(q => q.Id == op.Value))
                        return "One or more Equipments are invalid";

            if (Personnel.Count > 0)
                foreach (Option op in Personnel)
                    if (!await unitOfWork.Personnel.isExists(q => q.Id == op.Value))
                        return "One or more Personnel are invalid";

            if (Rooms.Count > 0)
                foreach (Option op in Rooms)
                    if (!await unitOfWork.Rooms.isExists(q => q.Id == op.Value))
                        return "One or more Rooms are invalid";

            if (Assignees.Count > 0)
                foreach (Option op in Assignees)
                    if (!await unitOfWork.TEMSUsers.isExists(q => q.Id == op.Value))
                        return "One or more Assignees are invalid";

            return null;
        }
    }
}
