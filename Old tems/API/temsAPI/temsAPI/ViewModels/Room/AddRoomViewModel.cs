using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.OtherEntities;

namespace temsAPI.ViewModels.Room
{
    public class AddRoomViewModel
    {
        public string Id { get; set; }
        public string Identifier { get; set; }
        public int Floor { get; set; }
        public string Description { get; set; }
        public List<Option> Labels { get; set; } = new List<Option>();
        public List<Option> Supervisories { get; set; } = new List<Option>();

        /// <summary>
        /// Validates an instance of AddRoomViewModel. Returns null if everything is ok,
        /// otherwise returns the error message.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public async Task<string> Validate(IUnitOfWork unitOfWork)
        {
            // If it's the update case
            Data.Entities.OtherEntities.Room roomToUpdate = null;
            if (Id != null)
            {
                roomToUpdate = (await unitOfWork.Rooms
                    .Find<Data.Entities.OtherEntities.Room>(q => q.Id == Id))
                    .FirstOrDefault();

                if (roomToUpdate == null)
                    return "Invalid id provided";
            }

            // Indentifier is not valid
            Identifier = Identifier?.Trim();
            if (String.IsNullOrEmpty(Identifier))
                return "Please, provide a valid room identifier";

            // This room already exists and it's not the udpate case
            if (Id == null)
                if (await unitOfWork.Rooms
                    .isExists(q => q.Identifier == Identifier && !q.IsArchieved))
                    return $"{Identifier} room already exists";

            // When it's the update case
            if (Id != null)
                if (await unitOfWork.Rooms
                    .isExists(q => q.Identifier == Identifier && !q.IsArchieved && q.Id != Id))
                    return $"This {Identifier} room already exists";

            // Invalid labels provided
            foreach (Option label in Labels)
                if (!await unitOfWork.RoomLabels
                    .isExists(q => q.Id == label.Value && !q.IsArchieved))
                    return "Invalid labels provided";

            // Invalid supervisories provided
            foreach (Option superv in Supervisories)
                if (!await unitOfWork.Personnel
                    .isExists(q => q.Id == superv.Value && !q.IsArchieved))
                    return "Invalid personnel assigned as supervisor.";

            return null;
        }

        public static AddRoomViewModel FromModel(Data.Entities.OtherEntities.Room room)
        {
            return new AddRoomViewModel
            {
                Id = room.Id,
                Identifier = room.Identifier,
                Description = room.Description,
                Floor = room.Floor ?? 0,
                Supervisories = room.Supervisories.Select(q => new Option
                {
                    Value = q.Id,
                    Label = q.Name
                }).ToList(),
                Labels = room.Labels.Select(q => new Option
                {
                    Value = q.Id,
                    Label = q.Name
                }).ToList(),
            };
        }
    }
}
