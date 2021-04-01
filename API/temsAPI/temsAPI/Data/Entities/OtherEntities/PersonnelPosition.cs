using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;

namespace temsAPI.Data.Entities.OtherEntities
{
    public class PersonnelPosition: IArchiveable, IIdentifiable
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime DateArchieved { get; set; }
        private bool isArchieved;
        public bool IsArchieved
        {
            get { return isArchieved; }
            set { isArchieved = value; DateArchieved = DateTime.Now; }
        }

        public virtual ICollection<Personnel> Personnel { get; set; } = new List<Personnel>();

        public string Identifier => Name;
    }
}
