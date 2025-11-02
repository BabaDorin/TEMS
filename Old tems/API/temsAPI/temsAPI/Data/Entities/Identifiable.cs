using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;

namespace temsAPI.Data.Entities
{
    public class Identifiable : IIdentifiable
    {
        public string Id { get; set; }

        public string Identifier { get; }

        public Identifiable(string id, string identifier)
        {
            Id = id;
            Identifier = identifier;
        }

        public Identifiable()
        {

        }
    }
}
