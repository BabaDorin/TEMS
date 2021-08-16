using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Asn1;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;

namespace temsAPI.Services.SICServices
{
    /// <summary>
    /// Contains the logic for making TEMS compatible with SIC
    /// Registers SIC Types
    /// </summary>
    public class SICIntegrationService
    {
        public List<string> SICProperties { get; }
        public List<string> SICTypes { get; private set; }

        private IUnitOfWork _unitOfWork;

        public SICIntegrationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            SICProperties = new List<string>()
            {
                "TeamViewerID", "TeamViewerPassword", "Manufacturer",
                "Name", "Architecture", "NumberOfCores", "ProcessorId",
                "L2CacheSize", "L3CacheSize", "ThreadCount", "MaxClockSpeed",
                "SocketDesignation", "AdapterRAM", "VideoModeDescription",
                "VideoProcessor",  "VideoModeDescription",  "MonitorManufacturer",
                "ScreenWidth", "RefreshRateInHz", "Product",
                "Description", "NetworkInterfaceType", "Speed", "Model",  "MaxOutputWattage",
                "Capacity", "Caption", "InterfaceType", "Size",
                "MediaType", "ConfiguredClockSpeed", "PartNumber"
            };

            SICTypes = new List<string>()
            {
                "CPU", "GPU", "Monitor", "Motherboard",
                "Network Interface", "PSU", "RAM", "Storage",
                "Computer",
            };
        }

        public async Task<string> PrepareDBForSICIntegration()
        {
            // In order to facilitate communication between TEMS and SIC, the first thing
            // to do is to add all of necessary types into TEMS Db (Computer, PowerSupply etc.).
            await AddNecessaryTypesAndProperties();
            return null;
        }

        /// <summary>
        /// Declares which are those datatypes, properties and Types that SIC needs to operate with and ensures
        /// their creation if case if they're absent
        /// </summary>
        /// <returns></returns>
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
                .Find<DataType>(q => q.Name.ToLower() == "bool"))
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
                    EditablePropertyInfo = false,
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "TeamViewer Access Password",
                    Name = "TeamViewerPassword",
                    EditablePropertyInfo = false,
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Manufacturer",
                    Name = "Manufacturer",
                    EditablePropertyInfo = false,
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Name",
                    Name = "Name",
                    EditablePropertyInfo = false,
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Architecture",
                    Name = "Architecture",
                    EditablePropertyInfo = false,
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = numberDT,
                    DisplayName = "Number of cores",
                    Name = "NumberOfCores",
                    EditablePropertyInfo = false,
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Processor ID",
                    Name = "ProcessorId",
                    EditablePropertyInfo = false,
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = numberDT,
                    DisplayName = "Level 2 Cache Size",
                    Name = "L2CacheSize",
                    EditablePropertyInfo = false,
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = numberDT,
                    DisplayName = "Level 3 Cache Size",
                    Name = "L3CacheSize",
                    EditablePropertyInfo = false,
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = numberDT,
                    DisplayName = "Thread Count",
                    Name = "ThreadCount",
                    EditablePropertyInfo = false,
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = numberDT,
                    DisplayName = "Maximum Clock Speed",
                    Name = "MaxClockSpeed",
                    EditablePropertyInfo = false,
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Socket Designation",
                    Name = "SocketDesignation",
                    EditablePropertyInfo = false,
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = numberDT,
                    DisplayName = "Adapter RAM",
                    Name = "AdapterRAM",
                    EditablePropertyInfo = false,
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Video Mode Description",
                    Name = "VideoModeDescription",
                    EditablePropertyInfo = false,
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Video Processor",
                    Name = "VideoProcessor",
                    EditablePropertyInfo = false,
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Video Mode Description",
                    Name = "VideoModeDescription",
                    EditablePropertyInfo = false,
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Video Memory Type",
                    Name = "VideoMemoryType",
                    EditablePropertyInfo = false,
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Monitor Manufacturer",
                    Name = "MonitorManufacturer",
                    EditablePropertyInfo = false,
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Screen Height",
                    Name = "ScreenHeight",
                    EditablePropertyInfo = false,
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Screen Width",
                    Name = "ScreenWidth",
                    EditablePropertyInfo = false,
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Refresh Rate (Hz)",
                    Name = "RefreshRateInHz",
                    EditablePropertyInfo = false,
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Product",
                    Name = "Product",
                    EditablePropertyInfo = false,
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Description",
                    Name = "Description",
                    EditablePropertyInfo = false,
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Network interface type",
                    Name = "NetworkInterfaceType",
                    EditablePropertyInfo = false,
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = numberDT,
                    DisplayName = "Speed",
                    Name = "Speed",
                    EditablePropertyInfo = false,
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Model",
                    Name = "Model",
                    EditablePropertyInfo = false,
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Maximum Output Wattage",
                    Name = "MaxOutputWattage",
                    EditablePropertyInfo = false,
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Capacity",
                    Name = "Capacity",
                    EditablePropertyInfo = false,
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Memory type",
                    Name = "MemoryType",
                    EditablePropertyInfo = false,
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Caption",
                    Name = "Caption",
                    EditablePropertyInfo = false,
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Interface Type",
                    Name = "InterfaceType",
                    EditablePropertyInfo = false,
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = numberDT,
                    DisplayName = "Size",
                    Name = "Size",
                    EditablePropertyInfo = false,
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Media Type",
                    Name = "MediaType",
                    EditablePropertyInfo = false,
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = numberDT,
                    DisplayName = "Configured Clock Speed",
                    Name = "ConfiguredClockSpeed",
                    EditablePropertyInfo = false,
                },
                new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    DataType = textDT,
                    DisplayName = "Partition number",
                    Name = "PartNumber",
                    EditablePropertyInfo = false,
                }
            };

            foreach (var prop in SICproperties)
            {
                await RegisterProperty(prop);
            }
            await _unitOfWork.Save();
            Debug.WriteLine("SIC Properties - Checked");

            // Sic Types
            var computerType = new EquipmentType
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Computer",
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
                    EditableTypeInfo = false,
                    Properties = await GetProperties("Manufacturer", "Name", "Architecture", "NumberOfCores", "ProcessorId", "Description", "L2CacheSize", "L3CacheSize", "ThreadCount", "MaxClockSpeed", "SocketDesignation"),
                },
                new EquipmentType
                {
                    Id = Guid.NewGuid().ToString(),
                    Parents = new List<EquipmentType>() {computerType},
                    Name = "GPU",
                    EditableTypeInfo = false,
                    Properties = await GetProperties("Name", "AdapterRAM", "VideoModeDescription", "VideoProcessor", "VideoMemoryType"),
                },
                new EquipmentType
                {
                    Id = Guid.NewGuid().ToString(),
                    Parents = new List<EquipmentType>() {computerType},
                    Name = "Monitor",
                    EditableTypeInfo = false,
                    Properties = await GetProperties("MonitorManufacturer", "Name", "ScreenHeight", "ScreenWidth", "RefreshRateInHz"),
                },
                new EquipmentType
                {
                    Id = Guid.NewGuid().ToString(),
                    Parents = new List<EquipmentType>() {computerType},
                    Name = "Motherboard",
                    EditableTypeInfo = false,
                    Properties = await GetProperties("Manufacturer", "Product"),
                },
                new EquipmentType
                {
                    Id = Guid.NewGuid().ToString(),
                    Parents = new List<EquipmentType>() {computerType},
                    Name = "Network Interface",
                    EditableTypeInfo = false,
                    Properties = await GetProperties("Name", "NetworkInterfaceType", "Speed"),
                },
                new EquipmentType
                {
                    Id = Guid.NewGuid().ToString(),
                    Parents = new List<EquipmentType>() {computerType},
                    Name = "PSU",
                    EditableTypeInfo = false,
                    Properties = await GetProperties("Model", "MaxOutputWattage", "SerialNumber"),
                },
                new EquipmentType
                {
                    Id = Guid.NewGuid().ToString(),
                    Parents = new List<EquipmentType>() {computerType},
                    Name = "RAM",
                    EditableTypeInfo = false,
                    Properties = await GetProperties("Manufacturer", "Capacity", "MemoryType", "ConfiguredClockSpeed", "PartNumber"),
                },
                new EquipmentType
                {
                    Id = Guid.NewGuid().ToString(),
                    Parents = new List<EquipmentType>() {computerType},
                    Name = "Storage",
                    EditableTypeInfo = false,
                    Properties = await GetProperties("Caption", "Model", "SerialNumber", "Size", "MediaType"),
                },
            };
            foreach (var type in SICequipmentTypes)
            {
                await RegisterType(type);
            }
            await _unitOfWork.Save();
        }

        private async Task<List<Property>> GetProperties(params string[] propertyNames)
        {
            return (await _unitOfWork.Properties
                .FindAll<Property>(q => propertyNames.Contains(q.Name)))
                .ToList();
        }

        /// <summary>
        /// Checks for conflicts and register the provided property.
        /// </summary>
        /// <param name="property"></param>
        /// <returns>True if registering process succedeed or the property already exists and has the same structure and the desired one.</returns>
        private async Task<bool> RegisterProperty(Property property)
        {
            // Check if another property with the same name already exists and differs from
            // our property
            // If so, check if the existing one is the same as the property that we're willing to register.
            // If not, return an error message that promts user to manualy remove the existing conflicting property 

            var conflictingProperty = (await _unitOfWork.Properties
                .Find<Property>(
                    where: q => q.Name.ToLower() == property.Name.ToLower(),
                    include: q => q.Include(q => q.DataType)))
                .FirstOrDefault();

            if (conflictingProperty != null)
            {
                if (conflictingProperty.DisplayName != property.DisplayName
                    || conflictingProperty.DataType != property.DataType)
                    throw new Exception($"Conflict: There already exists a property with the name: {conflictingProperty.Name} it's different from what SIC needs. Please, change the name of that property and run this process again. (If the type conflicting property is archived, remove it completely from the archive).");
                  
                return true;
            }

            await _unitOfWork.Properties.Create(property);
            return true;
        }

        /// <summary>
        /// Checks for conflicts and registers the provided equipment type, along with it's children.
        /// </summary>
        /// <param name="equipmentType"></param>
        /// <returns>True if registering process succedeed</returns>
        private async Task<bool> RegisterType(EquipmentType equipmentType)
        {
            // Check if type exists.
            // Throws an error if the type already exists and it differs from what 
            // SIC needs.

            var conflictingType = (await _unitOfWork.EquipmentTypes
                .Find<EquipmentType>(
                    where: q => q.Name.ToLower() == equipmentType.Name.ToLower(),
                    include: q => q.Include(q => q.Parents)
                                   .Include(q => q.Children)
                                   .Include(q => q.Properties)))
                .FirstOrDefault();

            if(conflictingType != null)
                throw new Exception($"Another type with the name: {equipmentType.Name} already exists. If SIC has not been integrated yet, please, change the name of that type and run this process again. (If the type conflicting type is archived, remove it completely from the archive).");

            await _unitOfWork.EquipmentTypes.Create(equipmentType);
            return true;
        }
    }
}
