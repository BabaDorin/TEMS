using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.ViewModels.Personnel;

namespace temsAPI.ViewModels.Profile
{
    public class ViewProfileViewModel
    {
        public string Id { get; set; }
        public string PersonnelId { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsArchieved { get; set; }
        public DateTime DateArchieved { get; set; }
        public DateTime DateRegistered { get; set; }
        public List<string> CreatedTickets { get; set; }
        public List<string> AssignedTickets { get; set; }
        public List<string> ClosedTickets { get; set; }
        public List<string> Announcements { get; set; }
        public List<string> AllocatedEquipment { get; set; }
        public List<string> RoomSupervisories { get; set; }
        public List<Option> AllocatedKey { get; set; }
    }
}
