using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Data.Entities.OtherEntities
{
    public class Status: IArchiveable, IIdentifiable
    {
        [Key] [MaxLength(150)]
        public string Id { get; set; }

        [MaxLength(150)]
        public string Name { get; set; }
        
        public int ImportanceIndex { get; set; } = Int32.MaxValue;

        public DateTime? DateArchieved { get; set; }
        private bool isArchieved;
        public bool IsArchieved
        {
            get
            {
                return isArchieved;
            }
            set
            {
                isArchieved = value;
                DateArchieved = (value)
                    ? DateTime.Now
                    : null;
            }
        }

#nullable enable
        [InverseProperty("ArchivedStatuses")]
        [ForeignKey("ArchievedById")]
        public TEMSUser? ArchievedBy { get; set; }
        public string? ArchievedById { get; set; }
#nullable disable

        public ICollection<Ticket> Tickets { get; set; }

        [NotMapped]
        public string Identifier => Name;
    }
}
