using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Data.Entities.CommunicationEntities
{
    public class FrequentTicketProblem
    {
        [Key]
        public string Id { get; set; }

        public string Problem { get; set; }
    }
}