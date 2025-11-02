using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIC_Parser.Models
{
    public class Motherboard : Device, ITEMSIndexable
    {
        public Computer Computer { get; set; }
        public string TEMSSerialNumber { get; set; }
        public string IdentifierPropertyName { get; } = "Product";

        public string Manufacturer { get; set; }
        public string Product { get; set; }
        public string SerialNumber { get; set; }

        // <summary>
        /// Build Motherboards's TEMSSerialNumber
        /// </summary>
        /// <param name="parameter">Not used. Specify null.</param>
        public void BuildIndex(object parameter)
        {
            TEMSSerialNumber = $"Motherboard_{SerialNumber}";
        }
    }
}
