using Domain.Abstractions;

namespace Domain
{
    public class BaseEntity : IIdentifiable
    {
        public string Id { get; set; }

        public string Identifier { get; }

        public BaseEntity(string id, string identifier)
        {
            Id = id;
            Identifier = identifier;
        }
    }
}
