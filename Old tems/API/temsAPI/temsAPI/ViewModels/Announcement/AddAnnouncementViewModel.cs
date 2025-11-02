using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.Announcement
{
    public class AddAnnouncementViewModel
    {
        public string Title { get; set; }
        public string Text { get; set; }

        public string Validate()
        {
            Title = Title?.Trim();
            Text = Text?.Trim();

            if (String.IsNullOrEmpty(Title))
                return "Announcement title is required";

            if (String.IsNullOrEmpty(Text))
                return "Announcement text is required";

            return null;
        }
    }
}
