using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.ViewModels.Personnel
{
    public class AddPersonnelViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public List<Option> Positions { get; set; }
        public Option User { get; set; }

        /// <summary>
        /// Validates an instance of AddPersonnelViewModel. Returns null if everything is ok,
        /// otherwise returns the error message.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        public async Task<string> Validate(IUnitOfWork unitOfWork)
        {
            // It's the udpate case and the provided id is invalid
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

            // The specified personnel - user connection is invalid
            // The user is already connected with a personnel
            if(User != null)
            {
                var user = (await unitOfWork.TEMSUsers
                    .Find<TEMSUser>(
                        where: q => q.Id == User.Value,
                        include: q => q.Include(q => q.Personnel)))
                    .FirstOrDefault();

                if (user == null)
                    return "Invalid user provided...";

                if (user.Personnel != null && user.PersonnelId != Id)
                    return "The specified is already associated with a personnel record";
            }

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
                }).ToList(),
                User = personnel.TEMSUser == null
                ? null
                : new Option
                {
                    Label = personnel.TEMSUser.FullName ?? personnel.TEMSUser.UserName,
                    Value = personnel.Id,
                }
            };
        }
    }
}
