using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.Profile
{
    public class ProfilePhotoViewModel
    {
        public string UserId { get; set; }
        public IFormFile Photo { get; set; }

        public string Validate()
        {
            if (string.IsNullOrEmpty(UserId))
                return "UserId is required";

            if (Photo == null || Photo.Length == 0)
                return "Photo is required";

            if (!new List<string>() { ".jpg", ".jpeg", ".gif", ".png" }.Contains(Path.GetExtension(Photo.FileName)))
                return "Invalid file type. Types supported: .jpg, .jpeg, .gif, .png";

            return null;
        }
    }
}
