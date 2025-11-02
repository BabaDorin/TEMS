using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIC_Parser.Models
{
    public class PSU : Device, ITEMSIndexable
    {
        public Computer Computer { get; set; }

        public string TEMSSerialNumber { get; set; }
        public string IdentifierPropertyName { get; } = "Model";

        public string Model { get; set; }
        public string MaxOutputWattage { get; set; }
        public string SerialNumber { get; set; }

        // <summary>
        /// Build PSU's TEMSSerialNumber
        /// </summary>
        /// <param name="parameter">unused parameter, specify null</param>
        public void BuildIndex(object parameter)
        {
            if (!String.IsNullOrEmpty(SerialNumber))
                TEMSSerialNumber = SerialNumber;
            else
                TEMSSerialNumber = $"PSU_{Computer.TEMSSerialNumber}";
        }
    }
}
