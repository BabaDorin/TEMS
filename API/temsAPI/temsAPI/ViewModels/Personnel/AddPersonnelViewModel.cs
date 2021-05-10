using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;

namespace temsAPI.ViewModels.Personnel
{
    public class AddPersonnelViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public List<Option> Positions { get; set; }

        /// <summary>
        /// Validates an instance of AddPersonnelViewModel. Returns null if everything is ok,
        /// otherwise returns the error message.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        public async Task<string> Validate(IUnitOfWork unitOfWork)
        {
            // It's the udpate case and the provided is is invalid
            Data.Entities.OtherEntities.Personnel personnelToUpdate = null;
            if (Id != null)
            {
                personnelToUpdate = (await unitOfWork.Personnel
                    .Find<Data.Entities.OtherEntities.Personnel>(q => q.Id == Id))
                    .FirstOrDefault();

                if (personnelToUpdate == null)
                    return "Invalid personnel Id";
            }

            // Invalid Name
            Name = Name?.Trim();
            if (String.IsNullOrEmpty(Name))
                return "Invalid personnel name provided";
            return null;
        }

        public static AddPersonnelViewModel FromModel(
            Data.Entities.OtherEntities.Personnel personnel)
        {
            return new AddPersonnelViewModel
            {
                Id = personnel.Id,
                Email = personnel.Email,
                Name = personnel.Name,
                PhoneNumber = personnel.PhoneNumber,
                Positions = personnel.Positions.Select(q => new Option
                {
                    Value = q.Id,
                    Label = q.Name
                }).ToList()
            };
        }
    }
}
