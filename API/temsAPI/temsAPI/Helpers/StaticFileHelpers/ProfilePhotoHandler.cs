using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Helpers.ImageProcessors;

namespace temsAPI.Helpers.StaticFileHelpers
{
    public class ProfilePhotoHandler : StaticFileHandler
    {
        public override string FolderPath { get; }

        public ProfilePhotoHandler()
        {
            FolderPath = Path.Combine(GetStaticFilesFolderPath(), "ProfilePhotos");
        }

        public async Task SaveProfilePhoto(IFormFile file, string fullPhotoName, string minifiedPhotoName)
        {
            // If it is a gif, then only the minified version is resized, the full one stays the same.
            // Following this approach, the minified version of the gif will be represented only by the first frame of the gif,
            // this is the behaviour that we're looking for.
            // For the full version, the original gif will be kept (it can't be larger than 1MB, this is specified by the controller
            // header, so we kind of don't have to worry about gif original size.

            // If it is an image, both versions will get resized

            string fullPhotoPath = Path.Combine(FolderPath, fullPhotoName);
            string minifiedPhotoPath = Path.Combine(FolderPath, minifiedPhotoName);

            if (Path.GetExtension(fullPhotoPath) == ".gif")
                using (var fileStream = new FileStream(fullPhotoPath, FileMode.Create))
                    await file.CopyToAsync(fileStream);

            // The image is converted to memorystream, then to full-size (640x640) and minified size (100x100) images
            // that are saved on the disk
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);

                ImageResizer resizer = new();
                
                if(Path.GetExtension(fullPhotoPath) != ".gif")
                {
                    var fullImage = resizer.ResizeImage(640, 640, ms);
                    fullImage.Save(fullPhotoPath);
                }

                var minifiedImage = resizer.ResizeImage(100, 100, ms);
                minifiedImage.Save(minifiedPhotoPath);
            }
        }

        public void RemoveProfilePhotoIfAny(TEMSUser user)
        {
            if(user.ProfilePhotoMinifiedFileName != null)
            {
                string pathFullImage = Path.Combine(FolderPath, user.ProfilePhotoFileName);
                if (File.Exists(pathFullImage))
                    File.Delete(pathFullImage);
            }

            if(user.ProfilePhotoMinifiedFileName != null)
            {
                string pathMinifiedImage = Path.Combine(FolderPath, user.ProfilePhotoMinifiedFileName);
                if (File.Exists(pathMinifiedImage))
                    File.Delete(pathMinifiedImage);
            }
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
