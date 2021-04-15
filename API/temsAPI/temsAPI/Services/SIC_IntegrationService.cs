using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Asn1;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;

namespace temsAPI.Services
{
    /// <summary>
    /// Contains the logic for making TEMS compatible with SIC
    /// </summary>
    public class SIC_IntegrationService
    {
        private IUnitOfWork _unitOfWork;

        public SIC_IntegrationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> PrepareDBForSICIntegration()
        {
            // In order to facilitate communication between TEMS and SIC, the first thing
            // to do is to add all of necessary types into TEMS Db (Computer, PowerSupply etc.).
            try
            {
                await AddNecessaryTypesAndProperties();
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ex.Message;
            }
        }

        private async Task AddNecessaryTypesAndProperties()
        {
            // DataTypes:
            var textDT = (await _unitOfWork.DataTypes
                .Find<DataType>(q => q.Name.ToLower() == "text"))
                .FirstOrDefault();
            var numberDT = (await _unitOfWork.DataTypes
                .Find<DataType>(q => q.Name.ToLower() == "number"))
                .FirstOrDefault();
            var booleanDT = (await _unitOfWork.DataTypes
                .Find<DataType>(q => q.Name.ToLower() == "boolean"))
                .FirstOrDefault();

            // SIC Properties:
            var SICproperties = new List<Property>()
            {
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "TeamViewer ID",
                    Name = "TeamViewerID",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "TeamViewer Access Password",
                    Name = "TeamViewerPassword",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Manufacturer",
                    Name = "Manufacturer",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Name",
                    Name = "Name",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Name",
                    Name = "Name",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Architecture",
                    Name = "Architecture",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = numberDT,
                    DisplayName = "Number of cores",
                    Name = "NumberOfCores",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Processor ID",
                    Name = "ProcessorId",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = numberDT,
                    DisplayName = "Level 2 Cache Size",
                    Name = "L2CacheSize",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = numberDT,
                    DisplayName = "Level 3 Cache Size",
                    Name = "L3CacheSize",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = numberDT,
                    DisplayName = "Thread Count",
                    Name = "ThreadCount",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = numberDT,
                    DisplayName = "Maximul Clock Speed",
                    Name = "MaxClockSpeed",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Socket Designation",
                    Name = "SocketDesignation",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = numberDT,
                    DisplayName = "Adapter RAM",
                    Name = "AdapterRAM",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Video Mode Description",
                    Name = "VideoModeDescription",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Video Processor",
                    Name = "VideoProcessor",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Video Mode Description",
                    Name = "VideoModeDescription",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Video Memory Type",
                    Name = "VideoMemoryType",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Monitor Manufacturer",
                    Name = "MonitorManufacturer",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Screen Height",
                    Name = "ScreenHeight",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Screen Width",
                    Name = "ScreenWidth",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Refresh Rate (Hz)",
                    Name = "RefreshRateInHz",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Product",
                    Name = "Product",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Description",
                    Name = "Description",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Network interface type",
                    Name = "NetworkInterfaceType",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = numberDT,
                    DisplayName = "Speed",
                    Name = "Speed",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Model",
                    Name = "Speed",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Maximum Output Wattage",
                    Name = "MaxOutputWattage",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Capacity",
                    Name = "Capacity",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Memory type",
                    Name = "MemoryType",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Caption",
                    Name = "Caption",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Interface Type",
                    Name = "InterfaceType",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = numberDT,
                    DisplayName = "Size",
                    Name = "Size",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Media Type",
                    Name = "MediaType",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = numberDT,
                    DisplayName = "Configured Clock Speed",
                    Name = "ConfiguredClockSpeed",
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Partition number",
                    Name = "PartNumber",
                }
            };

            SICproperties.ForEach(async property => await RegisterProperty(property));
            await _unitOfWork.Save();
            Debug.WriteLine("SIC Properties - Checked");

            // Sic Types
            var computerType = new EquipmentType
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Computer",
                Properties = await GetProperties("TeamViewerID", "TeamViewerPassword"),
            };
            await RegisterType(computerType);
            await _unitOfWork.Save();
            Debug.WriteLine("Computer type registered");

            var SICequipmentTypes = new List<EquipmentType>()
            {
                new EquipmentType
                {
                    Id = Guid.NewGuid().ToString(),
                    Parents = new List<EquipmentType>() {computerType},
                    Name = "CPU",
                    Properties = await GetProperties("Manufacturer", "Name", "Architecture", "NumberOfCores", "ProcessorId", "Description", "L2CacheSize", "L3CacheSize", "ThreadCount", "MaxClockSpeed", "SocketDesignation"),
                },
                new EquipmentType
                {
                    Id = Guid.NewGuid().ToString(),
                    Parents = new List<EquipmentType>() {computerType},
                    Name = "GPU",
                    Properties = await GetProperties("Name", "AdapterRAM", "VideoModeDescription", "VideoProcessor", "VideoMemoryType"),
                },
                new EquipmentType
                {
                    Id = Guid.NewGuid().ToString(),
                    Parents = new List<EquipmentType>() {computerType},
                    Name = "Monitor",
                    Properties = await GetProperties("MonitorManufacturer", "Name", "ScreenHeight", "ScreenWidth", "RefreshRateInHz"),
                },
                new EquipmentType
                {
                    Id = Guid.NewGuid().ToString(),
                    Parents = new List<EquipmentType>() {computerType},
                    Name = "Motherboard",
                    Properties = await GetProperties("Manufacturer", "Product"),
                },
                new EquipmentType
                {
                    Id = Guid.NewGuid().ToString(),
                    Parents = new List<EquipmentType>() {computerType},
                    Name = "Network Interface",
                    Properties = await GetProperties("Name", "NetworkInterfaceType", "Speed"),
                },
                new EquipmentType
                {
                    Id = Guid.NewGuid().ToString(),
                    Parents = new List<EquipmentType>() {computerType},
                    Name = "PSU",
                    Properties = await GetProperties("Model", "MaxOutputWattage", "SerialNumber"),
                },
                new EquipmentType
                {
                    Id = Guid.NewGuid().ToString(),
                    Parents = new List<EquipmentType>() {computerType},
                    Name = "RAM",
                    Properties = await GetProperties("Manufacturer", "Capacity", "MemoryType", "ConfiguredClockSpeed", "PartNumber"),
                },
                new EquipmentType
                {
                    Id = Guid.NewGuid().ToString(),
                    Parents = new List<EquipmentType>() {computerType},
                    Name = "Storage",
                    Properties = await GetProperties("Caption", "Model", "SerialNumber", "Size", "MediaType"),
                },
            };
            SICequipmentTypes.ForEach(async type => await RegisterType(type));
        }

        private async Task<List<Property>> GetProperties(params string[] propertyNames)
        {
            return (await _unitOfWork.Properties
                .FindAll<Property>(q => propertyNames.Contains(q.Name)))
                .ToList();
        }

        private async Task<bool> RegisterProperty(Property property)
        {
            // Check if another property with the same name already exists and differs from
            // our property

            var propertyAlreadyRegistered = (await _unitOfWork.Properties
                .Find<Property>(q => q.Name.ToLower() == property.Name.ToLower()))
                .FirstOrDefault();

            if (propertyAlreadyRegistered != null)
            {
                if (propertyAlreadyRegistered.DisplayName != property.DisplayName
                    || propertyAlreadyRegistered.DataType != property.DataType)
                    throw new Exception($"Another property with the name: {propertyAlreadyRegistered.Name} " +
                        $"already exists and it's different from what SIC needs. Please, change the name" +
                        $" of that property and run this process again.");
                else
                    return true;
            }

            await _unitOfWork.Properties.Create(property);
            return true;
        }

        private async Task<bool> RegisterType(EquipmentType equipmentType)
        {
            // Check if type exists.
            // Throws an error if the type aldreay exista and it differs from what 
            // SIC needs.

            var typeAlreadyRegistered = (await _unitOfWork.EquipmentTypes
                .Find<EquipmentType>(q => q.Name.ToLower() == equipmentType.Name.ToLower()))
                .FirstOrDefault();
            if(typeAlreadyRegistered != null)
            {
                if (typeAlreadyRegistered.Parents != equipmentType.Parents
                    || typeAlreadyRegistered.Children != equipmentType.Children
                    || typeAlreadyRegistered.Properties != equipmentType.Properties)
                    throw new Exception($"Another type with the name: {equipmentType.Name} " +
                        $"already exists and it's different from what SIC needs. Please, change the name" +
                        $" of that type and run this process again.");
                else
                    return true;
            }

            _unitOfWork.EquipmentTypes.Create(equipmentType);
            return true;
        }

    }
}
