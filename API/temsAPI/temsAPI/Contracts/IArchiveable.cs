using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Contracts
{
    public interface IArchiveable
    {
        bool IsArchieved { get; set; }
        DateTime? DateArchieved { get; set; }
    }
}
