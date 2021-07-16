using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Contracts
{
    public interface IIdentifiable
    {
        string Id { get; set; } 
        string Identifier { get; } 
    }
}
