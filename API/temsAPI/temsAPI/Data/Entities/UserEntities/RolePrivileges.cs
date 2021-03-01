using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Data.Entities.UserEntities
{
    public class RolePrivileges
    {
        [Key]
        public string ID { get; set; }

        [ForeignKey("RoleID")]
        public IdentityRole Role { get; set; }
        public string RoleID { get; set; }

        [ForeignKey("PrivilegeID")]
        public Privilege Privilege { get; set; }
        public string PrivilegeID { get; set; }
    }
}
