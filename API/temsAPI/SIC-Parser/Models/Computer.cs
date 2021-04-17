using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SIC_Parser.Models
{
    public class Computer
    {
        // follows singleton pattern
        private static Computer instance;

        private Computer()
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

        /// <summary>
        /// Returns the only instance of computer class (Singleton design pattern).
        /// </summary>
        /// <returns></returns>
        public static Computer GetInstance()
        {
            if (instance == null)
                instance = new Computer();

            return instance;
        }

        public string TEMSID { get; set; }
        public string Room { get; set; }
        public string Identifier { get; set; }
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
        public void DisplayData()
        {
            Debug.WriteLine("==============================================");
            Debug.WriteLine(TEMSID);
            CPUs.ForEach(s => s.DisplayData());
            GPUs.ForEach(s => s.DisplayData());
            PSUs.ForEach(s => s.DisplayData());
            Motherboards.ForEach(s => s.DisplayData());
            NetworkInterfaces.ForEach(s => s.DisplayData());
            RAMs.ForEach(s => s.DisplayData());
            Monitors.ForEach(s => s.DisplayData());
        }
    }
}
