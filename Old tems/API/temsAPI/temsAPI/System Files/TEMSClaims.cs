using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.System_Files
{
    public class TEMSClaims
    {
        public const string CAN_SEND_EMAILS = "Can send emails";
        public const string CAN_ALLOCATE_KEYS = "Can allocate keys";
        public const string CAN_MANAGE_ENTITIES = "Can manage Entities";
        public const string CAN_VIEW_ENTITIES = "Can view Entities";
        public const string CAN_MANAGE_ANNOUNCEMENTS = "Can manage announcements";
        public const string CAN_MANAGE_SYSTEM_CONFIGURATION = "Can manage system configuration";

        public static readonly IList<string> CLAIMS = new ReadOnlyCollection<string>
            (new List<String>
            {
                CAN_SEND_EMAILS,
                CAN_ALLOCATE_KEYS,
                CAN_MANAGE_ENTITIES,
                CAN_VIEW_ENTITIES,
                CAN_MANAGE_ANNOUNCEMENTS,
                CAN_MANAGE_SYSTEM_CONFIGURATION
            });
    }
}
