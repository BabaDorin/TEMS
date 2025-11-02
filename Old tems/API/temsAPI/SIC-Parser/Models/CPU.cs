using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIC_Parser.Models
{
    public class CPU : Device, ITEMSIndexable
    {
        public Computer Computer { get; set; }
        public string TEMSSerialNumber { get; set; }
        public string IdentifierPropertyName { get; } = "Name";

        public string Manufacturer { get; set; }
        public string Name { get; set; }
        public string Architecture { get; set; }
        public string NumberOfCores { get; set; }
        public string ProcessorId { get; set; }
        public string Description { get; set; }
        public string L2CacheSize { get; set; }
        public string L3CacheSize { get; set; }
        public string ThreadCount { get; set; }
        public string MaxClockSpeed { get; set; }
        public string SocketDesignation { get; set; }

        /// <summary>
        /// Build CPU's TEMSSerialNumber
        /// </summary>
        /// <param name="parameter">CPU's number of oder (1, 2, 3, 4 etc.)</param>
        public void BuildIndex(object parameter)
        {
            TEMSSerialNumber = $"CPU_{parameter}_{Computer.TEMSSerialNumber}";
        }
    }
}
