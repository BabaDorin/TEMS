using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace temsAPI.Data.Entities.UserEntities
{
    public class RolePrivileges
    {
        [Key] [MaxLength(150)]
        public string Id { get; set; }

        [ForeignKey("RoleID")]
        public IdentityRole Role { get; set; }
        public string RoleID { get; set; }

        [ForeignKey("PrivilegeID")]
        public Privilege Privilege { get; set; }

        [MaxLength(150)]
        public string PrivilegeID { get; set; }
    }
}
