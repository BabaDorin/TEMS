using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using temsAPI.Data.Entities.CommunicationEntities;

namespace temsAPI.Data.Entities.OtherEntities
{
    public class Status
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public int ImportanceIndex { get; set; } = Int32.MaxValue;
        public bool IsArchieved { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
    }
}
