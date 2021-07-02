using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.CommunicationEntities;

namespace temsAPI.Data.Entities.OtherEntities
{
    public class Label
    {
        [Key] [MaxLength(150)]
        public string Id { get; set; }

        [MaxLength(150)]
        public string Name { get; set; }
        
        [MaxLength(100)]
        public string ColorHex
        {
            get
            {
                
                return "RGB(" + Color.R.ToString() + ", " + Color.G.ToString() + ", " + Color.B.ToString() + ")";
            }
        }
        
        [NotMapped]
        public Color Color { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
    }
}
