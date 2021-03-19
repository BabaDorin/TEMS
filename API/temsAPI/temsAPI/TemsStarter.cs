using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI
{
    public class TemsStarter
    {
        public static void Start()
        {
            CreateStaticFilesDirectory();
        }

        private static void CreateStaticFilesDirectory()
        {
            Directory.CreateDirectory(
                    Directory.GetCurrentDirectory() +
                    "\\StaticFiles\\LibraryUploads"
                    );
        }
    }
}
