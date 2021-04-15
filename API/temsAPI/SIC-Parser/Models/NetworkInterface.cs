using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIC_Parser.Models
{
    public class NetworkInterface : Device
    {
        private string speed;

        public string PhysicalAddress { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string NetworkInterfaceType { get; set; }
        public string Speed
        {
            get { return speed; }
            set
            {
                speed = (value == "-1") ? "" : value;
            }
        }
    }
}
