using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.Library
{
    public class ViewLibraryItemViewModel
    {
        private string displayName;

        public string DisplayName
        {
            get { return String.IsNullOrEmpty(displayName)
                    ? String.Concat(
                        ActualName.Substring(0, ActualName.LastIndexOf('_')),
                        System.IO.Path.GetExtension(DbPath)
                        )
                    : displayName; }
            set { displayName = value; }
        }

        public string Id { get; set; }
        public string ActualName { get; set; }
        public string Description { get; set; }
        public DateTime DateUploaded { get; set; }
        public Option UploadedBy { get; set; }
        public string DbPath { get; set; }
        public double FileSize { get; set; }
        public int Downloads { get; set; }
    }
}
