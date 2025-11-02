using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.LibraryEntities;

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

        public static ViewLibraryItemViewModel FromModel(LibraryItem item)
        {
            return new ViewLibraryItemViewModel
            {
                Id = item.Id,
                ActualName = item.ActualName,
                DateUploaded = item.DateUploaded,
                DbPath = item.DbPath,
                Description = item.Description,
                DisplayName = item.DisplayName,
                Downloads = item.Downloads,
                FileSize = item.FileSize,
                UploadedBy = new Option
                {
                    Label = item.UploadedBy?.FullName ?? item.UploadedBy?.UserName,
                    Value = item.UploadedById
                }
            };
        }
    }
}
