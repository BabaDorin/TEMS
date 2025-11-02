using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Helpers.EMail
{
    public class EmailTo
    {
        public string Email { get; set; }
        public string Name { get; set; }

        public EmailTo()
        {

        }

        public EmailTo(string email, string name)
        {
            Email = email;
            Name = name;
        }

        public static EmailTo FromUser(TEMSUser user)
        {
            return new EmailTo(user.Email, user.FullName ?? user.UserName);
        }

        public static EmailTo FromPersonnel(Personnel personnel)
        {
            return new EmailTo(personnel.Email, personnel.Name);
        }
    }
}
