﻿using OfficeOpenXml;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace ReportGenerator.Helpers
{
    class ReportResourceHelper
    {
        /// <summary>
        /// Inserts an into the specified worksheet & position.
        /// </summary>
        /// <param name="imageName">file name (including extension) from the Resources folder</param>
        /// <param name="ws"></param>
        /// <param name="row">Row number (starts with 1)</param>
        /// <param name="column">Column number (starts with 1)</param>
        /// <returns>True if the image has been inserted successfuly</returns>
        public bool InsertImage(string imageName, ExcelWorksheet ws, int row, int column)
        {
            string resourcesPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources");

            string imagePath = Path.Combine(resourcesPath, imageName);
            if (!File.Exists(imagePath))
                return false;
            
            using (Image image = Image.FromFile(imagePath))
            {
                string imageNameWithoutExtension = Path.GetFileNameWithoutExtension(imageName);
                var excelImage = ws.Drawings.AddPicture(imageNameWithoutExtension, image);
                excelImage.SetPosition(row - 1, 0, column - 1, 0);
                return true;
            }
        }
    }
}
