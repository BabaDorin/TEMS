using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIC_Parser.Models
{
    public class Monitor : Device, ITEMSIndexable
    {
        public string TEMSSerialNumber { get; set; }
        public string IdentifierPropertyName { get; } = "Name";

        public string TEMSID { get; set; }
        public string SerialNumber { get; set; }
        public string MonitorManufacturer { get; set; }
        public string Name { get; set; } // Do not touch!
        public string ScreenHeight { get; set; }
        public string ScreenWidth { get; set; }
        public string RefreshRateInHz { get; set; }

        // <summary>
        /// Build Monitor's TEMSSerialNumber
        /// </summary>
        /// <param name="parameter">Not used. Specify null.</param>
        public void BuildIndex(object parameter)
        {
            TEMSSerialNumber = SerialNumber;
        }
    }
}
