using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.Announcement
{
    public class ViewAnnouncementViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public Option CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }

        public static ViewAnnouncementViewModel FromModel(
            Data.Entities.CommunicationEntities.Announcement announcement)
        {
            return new ViewAnnouncementViewModel
            {
                Id = announcement.Id,
                Text = announcement.Message,
                Title = announcement.Title,
                DateCreated = announcement.DateCreated,
                CreatedBy = announcement.Author == null
                ? null
                : new Option
                {
                    Value = announcement.Author.Id,
                    Label = announcement.Author.FullName ?? announcement.Author.UserName
                }
            };
        }
    }
}
