using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Contracts
{
    interface IPinable
    {
        public bool IsPinned { get; set; }
        public DateTime? DatePinned { get; set; }
    }
}
