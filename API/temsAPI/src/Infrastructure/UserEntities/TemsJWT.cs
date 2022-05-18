using System.ComponentModel.DataAnnotations;

namespace temsAPI.Data.Entities.UserEntities
{
    public class TemsJWT
    {
        [Key] [MaxLength(150)]
        public string Id { get; set; }

        [MaxLength(1000)]
        public string Content { get; set; }

        public DateTime ExpirationDate { get; set; }
    }
}
