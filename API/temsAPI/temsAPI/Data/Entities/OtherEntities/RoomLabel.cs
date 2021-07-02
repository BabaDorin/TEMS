using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Data.Entities.OtherEntities
{
    public class RoomLabel: IArchiveable, IIdentifiable
    {
        [Key] [MaxLength(150)]
        public string Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

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
        [InverseProperty("ArchivedRoomLabels")]
        [ForeignKey("ArchievedById")]
        public TEMSUser? ArchievedBy { get; set; }
        public string? ArchievedById { get; set; }
#nullable disable

        public ICollection<Room> Rooms { get; set; } = new List<Room>();

        [NotMapped]
        public string Identifier => Name;
    }
}
