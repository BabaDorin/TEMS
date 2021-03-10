using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Data.Entities.OtherEntities
{
    public class PersonnelPosition
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public double IsArchieved { get; set; }

        public virtual ICollection<Personnel> Personnel { get; set; } = new List<Personnel>();
    }
}
