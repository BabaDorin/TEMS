using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SIC_Parser.Models
{
    class ComputerValidator
    {
        /// <summary>
        /// Validates an instance of Computer. Returns null if everything is OK, otherwise - returns the error message.
        /// </summary>
        /// <returns></returns>
        public string Validate(Computer computer)
        {
            TrimStringProperties(computer);
            StringBuilder stringBuilder = new StringBuilder("");

            //TEMSID is empty
            if (string.IsNullOrEmpty(computer.TEMSID))
                stringBuilder.Append("TEMSID is empty\n");

            // Identifier is null
            if (string.IsNullOrEmpty(computer.Identifier))
                stringBuilder.Append("Identifier is empty\n");

            // CPUs ----------------------
            foreach (var cpu in computer.CPUs)
            {
                TrimStringProperties(cpu);

                // Name is empty (the identifier)
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

            foreach (var gpu in computer.GPUs)
            {
                TrimStringProperties(gpu);

                // Name is empty (the identifier)
                if (string.IsNullOrEmpty(gpu.Name))
                    stringBuilder.Append("GPU: Name is empty\n");

                // AdapterRam - not number
                if (!double.TryParse(gpu.AdapterRAM, out _))
                    stringBuilder.Append("GPU: AdapterRAM is not a number\n");
            }

            List<PSU> psusToRemove = new List<PSU>();
            foreach (var psu in computer.PSUs)
            {
                TrimStringProperties(psu);

                if (String.IsNullOrEmpty(psu.MaxOutputWattage) && String.IsNullOrEmpty(psu.Model) && String.IsNullOrEmpty(psu.MaxOutputWattage))
                {
                    psusToRemove.Add(psu);
                    continue;
                }

                // SerialNumber is empty
                if (string.IsNullOrEmpty(psu.SerialNumber))
                    stringBuilder.Append("PSU: SerialNumber is empty\n");

                // Model is empty (the identifier)
                if (string.IsNullOrEmpty(psu.Model))
                    stringBuilder.Append("PSU: Model is empty\n");

                if (psu.MaxOutputWattage == "")
                    psu.MaxOutputWattage = "0";

                // MaxOutputWattage is not a number
                if (!double.TryParse(psu.MaxOutputWattage, out _))
                    stringBuilder.Append("PSU: MaxOutputWattage is not a number\n");
            }
            computer.PSUs = computer.PSUs.Except(psusToRemove).ToList();

            if (computer.Motherboards.Count == 0)
                stringBuilder.Append("No motherboard identified ;(\n");
            foreach (var motherboard in computer.Motherboards)
            {
                TrimStringProperties(motherboard);

                // Product is empty (identifier)
                if (string.IsNullOrEmpty(motherboard.Product))
                    stringBuilder.Append("Motherboard: Product is empty\n");

                // Serial number is empty (Extremely important)
                if (string.IsNullOrEmpty(motherboard.SerialNumber))
                    stringBuilder.Append("Motherboard: SerialNumber is empty. (Motherboard serial number is computer's serial number. If SIC didn't find motherboard serial number, provide it manually (from cmd> wmic baseboard get serialnumber)\n");
            }

            foreach (var netIntf in computer.NetworkInterfaces)
            {
                TrimStringProperties(netIntf);

                // Description is empty (identifier)
                if (string.IsNullOrEmpty(netIntf.Description))
                    stringBuilder.Append("Network Interface: Description is empty\n");
            }

            foreach (var monitor in computer.Monitors)
            {
                TrimStringProperties(monitor);

                // SerialNumber or TEMSID - both are empty (identifiers)
                monitor.TEMSID = monitor.TEMSID.Trim();
                if (string.IsNullOrEmpty(monitor.TEMSID) && string.IsNullOrEmpty(monitor.SerialNumber))
                    stringBuilder.Append("Monitor: Provide TEMSID or SerialNumber, or both\n");

                // Name is empty (identifier)
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

            for (int i = 0; i < computer.RAMs.Count; i++)
            {
                var ram = computer.RAMs[i];

                TrimStringProperties(ram);

                // PartNumber is empty (identifier)
                if (string.IsNullOrEmpty(ram.PartNumber))
                    stringBuilder.Append("RAM: PartNumber is empty (important)\n");

                // Capacity is not a number
                if (!double.TryParse(ram.Capacity, out _))
                    stringBuilder.Append("RAM: Capacity is not a number\n");

                // CapacConfiguredClockSpeedity is not a number
                if (!double.TryParse(ram.ConfiguredClockSpeed, out _))
                    stringBuilder.Append("RAM: ConfiguredClockSpeed is not a number\n");

                ram.SerialNumber = $"RAM{i}_{ram.SerialNumber}_{computer.Motherboards[0].SerialNumber}";
            }

            foreach (var storage in computer.Storages)
            {
                TrimStringProperties(storage);

                // Caption serves as identifier
                if (storage.Caption == "" && storage.Model != "")
                    storage.Caption = storage.Model;

                // Identifier is empty (caption)
                if (string.IsNullOrEmpty(storage.Caption))
                    stringBuilder.Append("Storage: Caption is empty (identifier)\n");

                // Size is not a number
                if (storage.Size == "") storage.Size = "0";
                if (!double.TryParse(storage.Size, out _))
                    stringBuilder.Append("Storage: Size is not a number\n");
            }

            return (stringBuilder.ToString() == "") ? null : stringBuilder.ToString();
        }

        private void TrimStringProperties(object obj)
        {
            var properties = obj.GetType().GetProperties().Where(q => q.PropertyType == typeof(string));

            foreach(PropertyInfo prop in properties)
            {
                var propVal = prop.GetValue(obj);
                if (propVal == null)
                    continue;

                prop.SetValue(obj, propVal.ToString().Trim());
            }
        }
    }
}
