using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.ViewModels.Key;

namespace temsAPI.Data.Entities.KeyEntities
{
    public class Key: IArchiveableItem
    {
        [Key]
        public string Id { get; set; }

        public string Identifier { get; set; }
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
        public string? Description { get; set; }
        [ForeignKey("RoomId")]
        public Room? Room { get; set; }
        public string? RoomId { get; set; }

        [InverseProperty("ArchivedKeys")]
        [ForeignKey("ArchievedById")]
        public TEMSUser? ArchievedBy { get; set; }
        public string? ArchievedById { get; set; }
#nullable disable

        public virtual ICollection<KeyAllocation> KeyAllocations { get; set; } = new List<KeyAllocation>();

        public static Key FromAddKeyViewModel(AddKeyViewModel viewModel)
        {
            return new Key
            {
                Id = Guid.NewGuid().ToString(),
                Identifier = viewModel.Identifier,
                RoomId = viewModel.RoomId,
                Description = viewModel.Description
            };
        }
    }
}
