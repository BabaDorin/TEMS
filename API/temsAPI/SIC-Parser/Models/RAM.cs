using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIC_Parser.Models
{
    public class RAM : Device, ITEMSIndexable
    {
        public Computer Computer { get; set; }
        public string TEMSSerialNumber { get; set; }
        public string IdentifierPropertyName { get; } = "PartNumber";

        public string Manufacturer { get; set; }
        public string Capacity { get; set; }
        public string MemoryType { get; set; }
        public string SerialNumber { get; set; }
        public string ConfiguredClockSpeed { get; set; }
        public string PartNumber { get; set; }

        // <summary>
        /// Build RAM chip's TEMSSerialNumber
        /// </summary>
        /// <param name="parameter">Chip's number of oder (1, 2, 3, 4 etc.)</param>
        public void BuildIndex(object parameter)
        {
            TEMSSerialNumber = $"RAM_{parameter}_{SerialNumber}_{Computer.TEMSSerialNumber}";
        }
    }
}
