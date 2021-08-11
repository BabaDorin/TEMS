using System.Collections.Generic;
using System.Linq;
using temsAPI.Contracts;
using temsAPI.Helpers.EMail;

namespace temsAPI.ViewModels.Email
{
    public class EmailData
    {
        public string From { get; set; }
        public List<EmailTo> Recipients { get; set; } = new List<EmailTo>();
        public string Subject { get; set; }
        public string Text { get; set; }
        public IEnumerable<string> Attachments { get; set; } = new List<string>();

        public static EmailData FromNotification(
            INotification notification, 
            string from = null,
            List<EmailTo> recipients = null,
            string subject = null,
            string text = null)
        {
            return new EmailData
            {
                From = from ?? "TEMS CIH Cahul",
                Recipients = recipients ?? notification.GetUsers().Select(q => EmailTo.FromUser(q)).ToList(),
                Subject = subject ?? notification.Title,
                Text = text ?? notification.Message
            };
        }
    }
}
