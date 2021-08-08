using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Helpers.StaticFileHelpers;

namespace temsAPI.ViewModels.Profile
{
    public class ViewProfileViewModel
    {
        public string Id { get; set; }
        public Option Personnel { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool GetEmailNotifications { get; set; }
        public bool IsArchieved { get; set; }
        public List<string> Roles { get; set; }
        public DateTime DateArchieved { get; set; }
        public DateTime DateRegistered { get; set; }
        public string PhotoBase64 { get; set; }

        public static ViewProfileViewModel FromModel(TEMSUser user)
        {
            return new ViewProfileViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Username = user.UserName,
                Email = user.Email,
                IsArchieved = user.IsArchieved,
                DateRegistered = user.DateRegistered,
                GetEmailNotifications = user.GetEmailNotifications,
                PhoneNumber = user.PhoneNumber,
                Personnel = (user.Personnel == null)
                    ? null
                    : new Option
                    {
                        Value = user.PersonnelId,
                        Label = user.Personnel.Name,
                    },
                PhotoBase64 = user.ProfilePhotoFileName == null
                    ? null
                    : new ProfilePhotoHandler().GetFullProfilePhotoBase64(user)
            };
        }
    }
}
