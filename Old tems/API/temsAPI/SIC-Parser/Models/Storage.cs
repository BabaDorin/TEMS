using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIC_Parser.Models
{
    public class Storage : Device, ITEMSIndexable
    {
        public Computer Computer { get; set; }

        public string TEMSSerialNumber { get; set; }
        public string IdentifierPropertyName { get; } = "Caption";

        public string Caption { get; set; }
        public string Description { get; set; }
        public string InterfaceType { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public string Size { get; set; }
        public string MediaType { get; set; } // Do not touch!

        // <summary>
        /// Build Storage device's TEMSSerialNumber
        /// </summary>
        /// <param name="parameter">Storage device's number of oder (1, 2, 3, 4 etc.)</param>
        public void BuildIndex(object parameter)
        {
            TEMSSerialNumber = $"Storage_{parameter}_{SerialNumber}_{Computer.TEMSSerialNumber}";
        }
    }
}
