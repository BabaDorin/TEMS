using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Data.Entities.CommunicationEntities
{
    public class LogType
    {
        [Key]
        public string ID { get; set; }

        public string Type { get; set; }

        public virtual ICollection<Log> Logs { get; set; }
    }
}
