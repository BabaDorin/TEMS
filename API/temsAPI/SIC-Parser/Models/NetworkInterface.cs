using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIC_Parser.Models
{
    public class NetworkInterface : Device, ITEMSIndexable
    {
        public Computer Computer { get; set; }
        public string TEMSSerialNumber { get; set; }
        public string IdentifierPropertyName { get; } = "Description";

        public string PhysicalAddress { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string NetworkInterfaceType { get; set; }
        public string Speed { get; set; }

        // <summary>
        /// Build Network interface's TEMSSerialNumber
        /// </summary>
        /// <param name="parameter">Interface's number of oder (1, 2, 3, 4 etc.)</param>
        public void BuildIndex(object parameter)
        {
            TEMSSerialNumber = $"NetIntf_{parameter}_{Computer.TEMSSerialNumber}";
        }
    }
}
