using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

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


        /// <summary>
        /// Validates an instance of Computer. Returns null if everything is OK, otherwise - returns the error message.
        /// </summary>
        /// <returns></returns>
        public string Validate()
        {
            StringBuilder stringBuilder = new StringBuilder("");

            //TEMSID is empty
            TEMSID = TEMSID.Trim();
            if (string.IsNullOrEmpty(TEMSID))
                stringBuilder.Append("TEMSID is empty\n");

            // Identifier is null
            Identifier = Identifier.Trim();
            if (string.IsNullOrEmpty(Identifier))
                stringBuilder.Append("Identifier is empty\n");

            // CPUs ----------------------
            foreach(var cpu in CPUs)
            {
                // Name is empty (the identifier)
                cpu.Name = cpu.Name.Trim();
                if (string.IsNullOrEmpty(cpu.Name))
                    stringBuilder.Append("CPU: Name is empty\n");

                // Number of cores - not number
                if (!double.TryParse(cpu.NumberOfCores, out _))
                    stringBuilder.Append("CPU: Number of cores is not a number\n");

                // ThreadCount - not number
                if (!double.TryParse(cpu.ThreadCount, out _))
                    stringBuilder.Append("CPU: ThreadCount is not a number\n");

                // MaxClockSpeed - not number
                if (!double.TryParse(cpu.MaxClockSpeed, out _))
                    stringBuilder.Append("CPU: MaxClockSpeed is not a number\n");

                if (cpu.L2CacheSize == "") cpu.L2CacheSize = "0";
                if (cpu.L3CacheSize == "") cpu.L3CacheSize = "0";
                // L2 CacheSize or L3 cache size is empty
                if (!double.TryParse(cpu.L2CacheSize, out _) || !double.TryParse(cpu.L3CacheSize, out _))
                    stringBuilder.Append("CPU: Cache sizes are not numbers. Leave them empty if no cache size detected.\n");
            }

            foreach (var gpu in GPUs)
            {
                // Name is empty (the identifier)
                gpu.Name = gpu.Name.Trim();
                if (string.IsNullOrEmpty(gpu.Name))
                    stringBuilder.Append("GPU: Name is empty\n");

                // AdapterRam - not number
                if (!double.TryParse(gpu.AdapterRAM, out _))
                    stringBuilder.Append("GPU: AdapterRAM is not a number\n");
            }

            List<PSU> psusToRemove = new List<PSU>();
            foreach (var psu in PSUs)
            {
                if (psu.MaxOutputWattage == "" && psu.Model == "" && psu.MaxOutputWattage == "")
                {
                    psusToRemove.Add(psu);
                    continue;
                }

                // SerialNumber is empty
                psu.SerialNumber = psu.SerialNumber.Trim();
                if (string.IsNullOrEmpty(psu.SerialNumber))
                    stringBuilder.Append("PSU: SerialNumber is empty\n");

                // Model is empty (the identifier)
                psu.Model = psu.Model.Trim();
                if (string.IsNullOrEmpty(psu.Model))
                    stringBuilder.Append("PSU: Model is empty\n");

                if (psu.MaxOutputWattage == "")
                    psu.MaxOutputWattage = "0";

                // MaxOutputWattage is not a number
                if (!double.TryParse(psu.MaxOutputWattage, out _))
                    stringBuilder.Append("PSU: MaxOutputWattage is not a number\n");
            }
            PSUs = PSUs.Except(psusToRemove).ToList();

            if (Motherboards.Count == 0)
                stringBuilder.Append("No motherboard identified ;(\n");
            foreach (var motherboard in Motherboards)
            {
                // Product is empty (identifier)
                motherboard.Product = motherboard.Product.Trim();
                if (string.IsNullOrEmpty(motherboard.Product))
                    stringBuilder.Append("Motherboard: Product is empty\n");

                // Serial number is empty (Extremely important)
                motherboard.SerialNumber = motherboard.SerialNumber.Trim();
                if (string.IsNullOrEmpty(motherboard.SerialNumber))
                    stringBuilder.Append("Motherboard: SerialNumber is empty. (Motherboard serial number is computer's serial number. If SIC didn't find motherboard serial number, provide it manually (from cmd> wmic baseboard get serialnumber)\n");
            }

            foreach (var netIntf in NetworkInterfaces)
            {
                // Description is empty (identifier)
                netIntf.Description = netIntf.Description.Trim();
                if (string.IsNullOrEmpty(netIntf.Description))
                    stringBuilder.Append("Network Interface: Description is empty\n");
            }

            foreach(var monitor in Monitors)
            {
                // SerialNumber or TEMSID - both are empty (identifiers)
                monitor.SerialNumber = monitor.SerialNumber.Trim();
                monitor.TEMSID = monitor.TEMSID.Trim();
                if (string.IsNullOrEmpty(monitor.TEMSID) && string.IsNullOrEmpty(monitor.SerialNumber))
                    stringBuilder.Append("Monitor: Provide TEMSID or SerialNumber, or both\n");

                // Name is empty (identifier)
                monitor.Name = monitor.Name.Trim();
                if (string.IsNullOrEmpty(monitor.Name))
                    stringBuilder.Append("Monitor: Name is empty\n");

                // Refreshrate is not a number
                if (!double.TryParse(monitor.RefreshRateInHz, out _))
                    stringBuilder.Append("Monitor: RefreshRateInHz is not a number\n");

                if (monitor.ScreenHeight == "") monitor.ScreenHeight = "0";
                if (monitor.ScreenWidth == "") monitor.ScreenWidth = "0";
                if (!double.TryParse(monitor.ScreenHeight, out _))
                    stringBuilder.Append("Monitor: ScreenHeight is not a number\n");
                if (!double.TryParse(monitor.ScreenWidth, out _))
                    stringBuilder.Append("Monitor: ScreenWidth is not a number\n");
            }

            for (int i = 0; i < RAMs.Count; i++)
            {
                var ram = RAMs[i];

                // PartNumber is empty (identifier)
                ram.PartNumber = ram.PartNumber.Trim();
                if (string.IsNullOrEmpty(ram.PartNumber))
                    stringBuilder.Append("RAM: PartNumber is empty (important)\n");

                // Capacity is not a number
                if (!double.TryParse(ram.Capacity, out _))
                    stringBuilder.Append("RAM: Capacity is not a number\n");

                // CapacConfiguredClockSpeedity is not a number
                if (!double.TryParse(ram.ConfiguredClockSpeed, out _))
                    stringBuilder.Append("RAM: ConfiguredClockSpeed is not a number\n");

                ram.SerialNumber = $"RAM{i}_{ram.SerialNumber}_{Motherboards[0].SerialNumber}";
            }

            foreach (var storage in Storages)
            {
                // Caption serves as identifier
                if (storage.Caption == "" && storage.Model != "")
                    storage.Caption = storage.Model;

                // Identifier is empty (caption)
                storage.Caption = storage.Caption.Trim();
                if (string.IsNullOrEmpty(storage.Caption))
                    stringBuilder.Append("Storage: Caption is empty (identifier)\n");

                // Size is not a number
                if (storage.Size == "") storage.Size = "0";
                if (!double.TryParse(storage.Size, out _))
                    stringBuilder.Append("Storage: Size is not a number\n");
            }

            return (stringBuilder.ToString() == "") ? null : stringBuilder.ToString();
        }
    }

}
