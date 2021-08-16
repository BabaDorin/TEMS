using System;
using System.Collections.Generic;

namespace SIC_Parser.Models
{
    public class Computer
    {
        public string TEMSSerialNumber { get; set; }

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

        /// <summary>
        /// Assigns serial numbers to itself and it's children equipment
        /// </summary>
        public void AssignIndexes()
        {
            // Computer's serial number is motherboard's serial number
            // But motherboard's serial number will be something like Motherboard_[motherboardSerialNumber]
            if (Motherboards == null || Motherboards.Count == 0 || Motherboards[0].SerialNumber == null)
                throw new Exception("Before asigning an index for this computer instance, make sure the motherboard's serial number is valid");

            TEMSSerialNumber = Motherboards[0].SerialNumber;

            for (int i = 0; i < CPUs.Count; i++)
                CPUs[i].BuildIndex(i+1);

            for (int i = 0; i < GPUs.Count; i++)
                GPUs[i].BuildIndex(i+1);

            for (int i = 0; i < PSUs.Count; i++)
                PSUs[i].BuildIndex(null);

            for (int i = 0; i < Motherboards.Count; i++)
                Motherboards[i].BuildIndex(null);

            for (int i = 0; i < NetworkInterfaces.Count; i++)
                NetworkInterfaces[i].BuildIndex(i + 1);

            for (int i = 0; i < Monitors.Count; i++)
                Monitors[i].BuildIndex(null);

            for (int i = 0; i < RAMs.Count; i++)
                RAMs[i].BuildIndex(i+1);

            for (int i = 0; i < Storages.Count; i++)
                Storages[i].BuildIndex(i+1);
        }

        /// <summary>
        /// Validates the model agains data types mismatches or undesired null values
        /// </summary>
        /// <returns></returns>
        public string Validate()
        {
            return new ComputerValidator().Validate(this);
        }
    }
}
