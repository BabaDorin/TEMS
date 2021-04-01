using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.OtherEntities;

namespace temsAPI.Data.Entities.KeyEntities
{
    public class KeyAllocation: IArchiveableItem
    {
        [Key]
        public string Id { get; set; }

        [ForeignKey("PersonnelID")]
        public Personnel Personnel { get; set; }
        public string PersonnelID { get; set; }

        [ForeignKey("KeyID")]
        public Key Key { get; set; }
        public string KeyID { get; set; }
        public DateTime DateArchieved { get; set; }
        private bool isArchieved;
        public bool IsArchieved
        {
            get { return isArchieved; }
            set { isArchieved = value; DateArchieved = DateTime.Now; }
        }


        public DateTime DateAllocated { get; set; }

#nullable enable
        public DateTime? DateReturned { get; set; }
#nullable disable

        [NotMapped]
        public string Identifier => $"Key: {Key?.Identifier}, Personnel: {Personnel?.Name}";

    }
}
