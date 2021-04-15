using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SIC_Parser.Models
{
    public class GPU : Device
    {
        private string videoMemoryType;

        public string Name { get; set; }
        public string AdapterRAM { get; set; }
        public string VideoModeDescription { get; set; }
        public string VideoProcessor { get; set; }
        public string VideoMemoryType
        {
            get
            {
                return videoMemoryType;
            }
            set
            {
                switch (value)
                {
                    case "1": videoMemoryType = "Other"; break; 
                    case "2": videoMemoryType = ""; break; 
                    case "3": videoMemoryType = "VRAM "; break; 
                    case "4": videoMemoryType = "DRAM "; break; 
                    case "5": videoMemoryType = "SRAM "; break; 
                    case "6": videoMemoryType = "WRAM "; break; 
                    case "7": videoMemoryType = "EDO RAM"; break; 
                    case "8": videoMemoryType = "Burst Synchronous DRAM"; break; 
                    case "9": videoMemoryType = "Pipelined Burst SRAM"; break; 
                    case "10": videoMemoryType = "CDRAM"; break; 
                    case "11": videoMemoryType = "3DRAM"; break;
                    case "12": videoMemoryType = "SDRAM"; break;
                    case "13": videoMemoryType = "SGRAM "; break;
                    default: videoMemoryType = ""; break;
                }
            }
        }
    }
}
