using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.CommunicationEntities;

namespace temsAPI.Data.Factories.LogFactories
{
    interface ILogFactory
    {
        Log Create();
    }
}
