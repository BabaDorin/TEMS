using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIC_Parser.Models
{
    public class RAM : Device
    {
        private string memoryType;

        public string Manufacturer { get; set; }
        public string Capacity { get; set; }
        public string MemoryType
        {
            get { return memoryType; }
            set
            {
                switch (value)
                {
                    case "1": memoryType = "Other"; break;
                    case "2": memoryType = "DRAM"; break;
                    case "3": memoryType = "Synchronous DRAM"; break;
                    case "4": memoryType = "Cache DRAM"; break;
                    case "5": memoryType = "EDO"; break;
                    case "6": memoryType = "EDRAM"; break;
                    case "7": memoryType = "VRAM"; break;
                    case "8": memoryType = "SRAM"; break;
                    case "9": memoryType = "RAM"; break;
                    case "10": memoryType = "ROM"; break;
                    case "11": memoryType = "Flash"; break;
                    case "12": memoryType = "EEPROM"; break;
                    case "13": memoryType = "FEPROM"; break;
                    case "14": memoryType = "EPROM"; break;
                    case "15": memoryType = "CDRAM"; break;
                    case "16": memoryType = "3DRAM"; break;
                    case "17": memoryType = "SDRAM"; break;
                    case "18": memoryType = "SGRAM"; break;
                    case "19": memoryType = "RDRAM"; break;
                    case "20": memoryType = "DDR"; break;
                    case "21": memoryType = "DDR2"; break;
                    case "22": memoryType = "DDR2 FB-DIMM"; break;
                    case "23": memoryType = "DDR2—FB-DIMM"; break;
                    case "24": memoryType = "DDR3"; break;
                    case "25": memoryType = "FBD2"; break;
                    case "26": memoryType = "DDR4"; break;
                    default: memoryType = ""; break;
                }
            }
        }
        public string SerialNumber { get; set; }
        public string ConfiguredClockSpeed { get; set; }
        public string PartNumber { get; set; }
    }
}
