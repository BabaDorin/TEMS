﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Helpers.StaticFileHelpers
{
    public class ProfilePhotoHandler : StaticFileHandler
    {
        public override string FolderPath { get; }

        public ProfilePhotoHandler()
        {
            FolderPath = Path.Combine(GetStaticFilesFolderPath(), "ProfilePhotos");
        }

        public async Task SaveProfilePhoto(IFormFile file, string fileName)
        {
            string filePath = Path.Combine(FolderPath, fileName);

            using (Stream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
        }

        public void RemoveProfilePhoto(TEMSUser user)
        {
            string fullPath = Path.Combine(FolderPath, user.ProfilePhotoFileName);
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

        public string GetProfilePhotoBase64(TEMSUser user)
        {
            if (user.ProfilePhotoFileName == null)
                return null;

            string path = Path.Combine(FolderPath, user.ProfilePhotoFileName);

            if (!File.Exists(path))
                return null;

            byte[] bytes = File.ReadAllBytes(path);
            var b64Data = Convert.ToBase64String(bytes);
            
            string imageType = Path.GetExtension(user.ProfilePhotoFileName).Substring(1);

            return String.Format($"data:image/{imageType};base64,{b64Data}");
        }
    }
}