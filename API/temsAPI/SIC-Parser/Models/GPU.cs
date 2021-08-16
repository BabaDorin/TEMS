using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SIC_Parser.Models
{
    public class GPU : Device, ITEMSIndexable
    {
        public Computer Computer { get; set; }
        public string TEMSSerialNumber { get; set; }
        public string IdentifierPropertyName { get; } = "Name";

        public string Name { get; set; }
        public string AdapterRAM { get; set; }
        public string VideoModeDescription { get; set; }
        public string VideoProcessor { get; set; }
        public string VideoMemoryType { get; set; }

        // <summary>
        /// Build GPU's TEMSSerialNumber
        /// </summary>
        /// <param name="parameter">GPU's number of oder (1, 2, 3, 4 etc.)</param>
        public void BuildIndex(object parameter)
        {
            TEMSSerialNumber = $"GPU_{parameter}_{Computer.TEMSSerialNumber}";
        }
    }
}
