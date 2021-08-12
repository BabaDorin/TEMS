using System.Collections.Generic;

namespace SIC_Parser.Models
{
    public class Computer
    {
        public Computer()
        {
            TEMSID = "";
            CPUs = new List<CPU>();
            GPUs = new List<GPU>();
            Motherboards = new List<Motherboard>();
            NetworkInterfaces = new List<NetworkInterface>();
            RAMs = new List<RAM>();
            Storages = new List<Storage>();
            PSUs = new List<PSU>();
            Monitors = new List<Monitor>();
        }

        public string TEMSID { get; set; }
        public string Identifier { get; set; }
        public bool IsUsed { get; set; }
        public bool IsDefect { get; set; }
        public string TeamViewerID { get; set; }
        public string TeamViewerPassword { get; set; }
        public string Description { get; set; }

        // All of the components are stored in generic lists, which makes 
        // generic functions for Reading / wrinting simpler and easier to implement.
        public List<CPU> CPUs { get; set; }
        public List<GPU> GPUs { get; set; }
        public List<PSU> PSUs { get; set; }
        public List<Motherboard> Motherboards { get; set; } 
        public List<NetworkInterface> NetworkInterfaces { get; set; }
        public List<Monitor> Monitors { get; set; }
        public List<RAM> RAMs { get; set; }
        public List<Storage> Storages { get; set; }

        public string Validate()
        {
            return new ComputerValidator().Validate(this);
        }
    }
}
