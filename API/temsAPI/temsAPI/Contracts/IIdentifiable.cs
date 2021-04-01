using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Contracts
{
    interface IIdentifiable
    {
        public string Identifier { get; }
    }
}
