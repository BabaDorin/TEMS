using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Data.Entities.EquipmentEntities
{
    [Index(nameof(EquipmentDefinitionID))]
    public class EquipmentSpecifications: IArchiveable
    {
        // Which properties relates to which equipment types.

        [Key] [MaxLength(150)]
        public string Id { get; set; }

        [ForeignKey("EquipmentDefinitionID")]
        public EquipmentDefinition EquipmentDefinition { get; set; }

        [MaxLength(150)]
        public string EquipmentDefinitionID { get; set; }


        [ForeignKey("PropertyID")]
        public Property Property { get; set; }

        [MaxLength(150)]
        public string PropertyID { get; set; }

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
        [MaxLength(1500)]
        public string? Value { get; set; }

        [InverseProperty("ArchivedSpecifications")]
        [ForeignKey("ArchievedById")]
        public TEMSUser? ArchievedBy { get; set; }
        public string? ArchievedById { get; set; }
#nullable disable
    }
}
