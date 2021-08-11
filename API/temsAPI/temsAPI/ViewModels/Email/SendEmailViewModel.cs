using System;
using System.Collections.Generic;
using temsAPI.Helpers.ReusableSnippets;

namespace temsAPI.ViewModels.Email
{
    public class SendEmailViewModel
    {
        public string From { get; set; }
        public List<string> Recipients { get; set; } = new();
        public string Subject { get; set; }
        public string Text { get; set; }

        public string Validate()
        {
            if (String.IsNullOrEmpty(From))
                return "'From' field not specified";

            if (String.IsNullOrEmpty(Subject))
                return "'Subject' field not specified";

            if (String.IsNullOrEmpty(Text))
                return "'Text' field not specified";

            if (Recipients.IsNullOrEmpty())
                return "No recipients were specified";

            return null;
        }
    }
}
