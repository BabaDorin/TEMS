using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.CommunicationEntities;

namespace temsAPI.Data.Entities.OtherEntities
{
    public class Status: IArchiveable, IIdentifiable
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public int ImportanceIndex { get; set; } = Int32.MaxValue;
        public DateTime DateArchieved { get; set; }
        private bool isArchieved;
        public bool IsArchieved
        {
            get { return isArchieved; }
            set { isArchieved = value; DateArchieved = DateTime.Now; }
        }

        public ICollection<Ticket> Tickets { get; set; }

        [NotMapped]
        public string Identifier => Name;
    }
}
