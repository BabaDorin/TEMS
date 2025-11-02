using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Contracts
{
    public interface IArchiveable
    {
        bool IsArchieved { get; set; }
        DateTime? DateArchieved { get; set; }
        TEMSUser ArchievedBy { get; set; }
    }
}
