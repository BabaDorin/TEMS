using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.System_Files.TEMSInitializer
{
    public class StaticFilesFolderInitializer : IInitializerAction
    {
        
        /// <summary>
        /// Ensures the correctness of Static Files folder structure
        /// </summary>
        public void Start()
        {
            string staticFilesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles");

            CreateFolderIfDoesntExist(staticFilesDirectory);
            CreateFolderIfDoesntExist(Path.Combine(staticFilesDirectory, "GeneratedReports"));
            CreateFolderIfDoesntExist(Path.Combine(staticFilesDirectory, "LibraryUploads"));
            CreateFolderIfDoesntExist(Path.Combine(staticFilesDirectory, "TempReports"));
        }

        private void CreateFolderIfDoesntExist(string folderPath)
        {
            if (Directory.Exists(folderPath))
                return;

            Directory.CreateDirectory(folderPath);
        }
    }
}
