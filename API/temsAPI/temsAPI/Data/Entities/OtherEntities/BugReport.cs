using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.ViewModels.BugReport;

namespace temsAPI.Data.Entities.OtherEntities
{
    public class BugReport
    {
        [Key]
        public string Id { get; set; }

        [MaxLength(150)]
        public string ReportType { get; set; }
        
        [MaxLength(3000)]
        public string Description { get; set; }

        public DateTime DateCreated { get; set; }

#nullable enable

        [MaxLength(10000)]
        private string? Attachments { get; set; }

        [ForeignKey(nameof(CreatedByID))]
        public TEMSUser? CreatedBy { get; set; }
        public string? CreatedByID { get; set; }

#nullable disable

        public void SetAttachments(List<string> attachmentUris)
        {
            Attachments = String.Join(',', attachmentUris);
        }

        public List<string> GetAttachmentUris()
        {
            if (String.IsNullOrEmpty(Attachments))
                return new List<string>();

            return Attachments.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }
}
