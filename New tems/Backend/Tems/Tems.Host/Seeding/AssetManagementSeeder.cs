using AssetManagement.Infrastructure.Entities;
using MongoDB.Driver;

namespace Tems.Host.Seeding;

public class AssetManagementSeeder(IMongoDatabase database, ILogger<AssetManagementSeeder> logger)
{
    private readonly IMongoCollection<AssetProperty> _properties = database.GetCollection<AssetProperty>("asset_properties");
    private readonly IMongoCollection<AssetType> _assetTypes = database.GetCollection<AssetType>("asset_types");
    private readonly IMongoCollection<AssetDefinition> _assetDefinitions = database.GetCollection<AssetDefinition>("asset_definitions");
    private readonly IMongoCollection<Asset> _assets = database.GetCollection<Asset>("assets");

    public async Task SeedAsync()
    {
        logger.LogInformation("Seeding Asset Management data...");

        await SeedPropertiesAsync();
        await SeedAssetTypesAsync();
        await SeedAssetDefinitionsAsync();
        await SeedAssetsAsync();

        logger.LogInformation("Asset Management seeding completed.");
    }

    private async Task SeedPropertiesAsync()
    {
        var count = await _properties.CountDocumentsAsync(FilterDefinition<AssetProperty>.Empty);
        if (count > 0)
        {
            logger.LogInformation("Asset properties already seeded. Skipping.");
            return;
        }

        logger.LogInformation("Seeding asset properties...");

        var properties = new List<AssetProperty>
        {
            // General Properties
            new() { PropertyId = "prop_cpu", Name = "CPU", Description = "Central Processing Unit model", DataType = "string", Category = "Hardware" },
            new() { PropertyId = "prop_ram", Name = "RAM", Description = "Memory capacity", DataType = "number", Category = "Hardware", Unit = "GB" },
            new() { PropertyId = "prop_storage", Name = "Storage", Description = "Storage capacity", DataType = "number", Category = "Hardware", Unit = "GB" },
            new() { PropertyId = "prop_screen_size", Name = "Screen Size", Description = "Display size", DataType = "number", Category = "Display", Unit = "inches" },
            new() { PropertyId = "prop_resolution", Name = "Resolution", Description = "Display resolution", DataType = "string", Category = "Display" },
            new() { PropertyId = "prop_os", Name = "Operating System", Description = "Installed operating system", DataType = "enum", Category = "Software", 
                EnumValues = ["Windows 11", "Windows 10", "macOS Sonoma", "macOS Ventura", "Ubuntu 22.04", "Ubuntu 20.04"] },
            new() { PropertyId = "prop_ip_address", Name = "IP Address", Description = "Network IP address", DataType = "string", Category = "Network" },
            new() { PropertyId = "prop_mac_address", Name = "MAC Address", Description = "Network MAC address", DataType = "string", Category = "Network" },
            new() { PropertyId = "prop_hostname", Name = "Hostname", Description = "Network hostname", DataType = "string", Category = "Network" },
            new() { PropertyId = "prop_warranty_expiry", Name = "Warranty Expiration", Description = "Warranty end date", DataType = "date", Category = "Lifecycle" },
            
            // Printer Properties
            new() { PropertyId = "prop_print_speed", Name = "Print Speed", Description = "Pages per minute", DataType = "number", Category = "Performance", Unit = "ppm" },
            new() { PropertyId = "prop_print_technology", Name = "Print Technology", Description = "Printing technology", DataType = "enum", Category = "Hardware",
                EnumValues = ["Laser", "Inkjet", "Thermal", "Dot Matrix"] },
            new() { PropertyId = "prop_color_support", Name = "Color Support", Description = "Color printing capability", DataType = "boolean", Category = "Features" },
            new() { PropertyId = "prop_duplex", Name = "Duplex Printing", Description = "Automatic two-sided printing", DataType = "boolean", Category = "Features" },
            
            // Monitor Properties
            new() { PropertyId = "prop_panel_type", Name = "Panel Type", Description = "Display panel technology", DataType = "enum", Category = "Display",
                EnumValues = ["IPS", "TN", "VA", "OLED"] },
            new() { PropertyId = "prop_refresh_rate", Name = "Refresh Rate", Description = "Screen refresh rate", DataType = "number", Category = "Display", Unit = "Hz" },
            
            // Network Equipment
            new() { PropertyId = "prop_port_count", Name = "Port Count", Description = "Number of network ports", DataType = "number", Category = "Network" },
            new() { PropertyId = "prop_poe_support", Name = "PoE Support", Description = "Power over Ethernet", DataType = "boolean", Category = "Features" },
            new() { PropertyId = "prop_speed", Name = "Speed", Description = "Network speed", DataType = "enum", Category = "Performance",
                EnumValues = ["10 Mbps", "100 Mbps", "1 Gbps", "10 Gbps"] },
            
            // Phone Properties  
            new() { PropertyId = "prop_voip_support", Name = "VoIP Support", Description = "Voice over IP capability", DataType = "boolean", Category = "Features" },
            new() { PropertyId = "prop_poe_class", Name = "PoE Class", Description = "Power over Ethernet class", DataType = "enum", Category = "Hardware",
                EnumValues = ["Class 1", "Class 2", "Class 3", "Class 4"] },
                
            // Furniture Properties
            new() { PropertyId = "prop_dimensions", Name = "Dimensions", Description = "Physical dimensions", DataType = "string", Category = "Physical" },
            new() { PropertyId = "prop_material", Name = "Material", Description = "Construction material", DataType = "enum", Category = "Physical",
                EnumValues = ["Wood", "Metal", "Plastic", "Glass", "Composite"] },
            new() { PropertyId = "prop_color", Name = "Color", Description = "Color or finish", DataType = "string", Category = "Appearance" },
            new() { PropertyId = "prop_adjustable", Name = "Adjustable", Description = "Height adjustable", DataType = "boolean", Category = "Features" },
        };

        await _properties.InsertManyAsync(properties);
        logger.LogInformation("Seeded {Count} asset properties.", properties.Count);
    }

    private async Task SeedAssetTypesAsync()
    {
        var count = await _assetTypes.CountDocumentsAsync(FilterDefinition<AssetType>.Empty);
        if (count > 0)
        {
            logger.LogInformation("Asset types already seeded. Skipping.");
            return;
        }

        logger.LogInformation("Seeding asset types...");

        var assetTypes = new List<AssetType>
        {
            // Root types
            new()
            {
                Id = "type_computer",
                Name = "Computer",
                Description = "Computing devices including desktops and laptops",
                Properties =
                [
                    new AssetTypeProperty { PropertyId = "prop_cpu", Name = "CPU", DataType = "string", Required = true, DisplayOrder = 1 },
                    new AssetTypeProperty { PropertyId = "prop_ram", Name = "RAM", DataType = "number", Required = true, DisplayOrder = 2 },
                    new AssetTypeProperty { PropertyId = "prop_storage", Name = "Storage", DataType = "number", Required = true, DisplayOrder = 3 },
                    new AssetTypeProperty { PropertyId = "prop_os", Name = "Operating System", DataType = "enum", Required = true, DisplayOrder = 4 },
                    new AssetTypeProperty { PropertyId = "prop_hostname", Name = "Hostname", DataType = "string", Required = false, DisplayOrder = 5 },
                    new AssetTypeProperty { PropertyId = "prop_ip_address", Name = "IP Address", DataType = "string", Required = false, DisplayOrder = 6 },
                    new AssetTypeProperty { PropertyId = "prop_mac_address", Name = "MAC Address", DataType = "string", Required = false, DisplayOrder = 7 },
                    new AssetTypeProperty { PropertyId = "prop_warranty_expiry", Name = "Warranty Expiration", DataType = "date", Required = false, DisplayOrder = 8 },
                ],
                CreatedBy = "system"
            },
            new()
            {
                Id = "type_laptop",
                Name = "Laptop",
                Description = "Portable computer",
                ParentTypeId = "type_computer",
                Properties =
                [
                    new AssetTypeProperty { PropertyId = "prop_screen_size", Name = "Screen Size", DataType = "number", Required = true, DisplayOrder = 8 },
                    new AssetTypeProperty { PropertyId = "prop_resolution", Name = "Resolution", DataType = "string", Required = false, DisplayOrder = 9 },
                ],
                CreatedBy = "system"
            },
            new()
            {
                Id = "type_desktop",
                Name = "Desktop",
                Description = "Desktop computer workstation",
                ParentTypeId = "type_computer",
                Properties = [],
                CreatedBy = "system"
            },
            new()
            {
                Id = "type_monitor",
                Name = "Monitor",
                Description = "Display monitor",
                Properties =
                [
                    new AssetTypeProperty { PropertyId = "prop_screen_size", Name = "Screen Size", DataType = "number", Required = true, DisplayOrder = 1 },
                    new AssetTypeProperty { PropertyId = "prop_resolution", Name = "Resolution", DataType = "string", Required = true, DisplayOrder = 2 },
                    new AssetTypeProperty { PropertyId = "prop_panel_type", Name = "Panel Type", DataType = "enum", Required = false, DisplayOrder = 3 },
                    new AssetTypeProperty { PropertyId = "prop_refresh_rate", Name = "Refresh Rate", DataType = "number", Required = false, DisplayOrder = 4 },
                    new AssetTypeProperty { PropertyId = "prop_warranty_expiry", Name = "Warranty Expiration", DataType = "date", Required = false, DisplayOrder = 5 },
                ],
                CreatedBy = "system"
            },
            new()
            {
                Id = "type_printer",
                Name = "Printer",
                Description = "Printing device",
                Properties =
                [
                    new AssetTypeProperty { PropertyId = "prop_print_technology", Name = "Print Technology", DataType = "enum", Required = true, DisplayOrder = 1 },
                    new AssetTypeProperty { PropertyId = "prop_color_support", Name = "Color Support", DataType = "boolean", Required = true, DisplayOrder = 2 },
                    new AssetTypeProperty { PropertyId = "prop_duplex", Name = "Duplex Printing", DataType = "boolean", Required = false, DisplayOrder = 3 },
                    new AssetTypeProperty { PropertyId = "prop_print_speed", Name = "Print Speed", DataType = "number", Required = false, DisplayOrder = 4 },
                    new AssetTypeProperty { PropertyId = "prop_ip_address", Name = "IP Address", DataType = "string", Required = false, DisplayOrder = 5 },
                    new AssetTypeProperty { PropertyId = "prop_warranty_expiry", Name = "Warranty Expiration", DataType = "date", Required = false, DisplayOrder = 6 },
                ],
                CreatedBy = "system"
            },
            new()
            {
                Id = "type_network_switch",
                Name = "Network Switch",
                Description = "Network switching equipment",
                Properties =
                [
                    new AssetTypeProperty { PropertyId = "prop_port_count", Name = "Port Count", DataType = "number", Required = true, DisplayOrder = 1 },
                    new AssetTypeProperty { PropertyId = "prop_speed", Name = "Speed", DataType = "enum", Required = true, DisplayOrder = 2 },
                    new AssetTypeProperty { PropertyId = "prop_poe_support", Name = "PoE Support", DataType = "boolean", Required = false, DisplayOrder = 3 },
                    new AssetTypeProperty { PropertyId = "prop_ip_address", Name = "IP Address", DataType = "string", Required = false, DisplayOrder = 4 },
                    new AssetTypeProperty { PropertyId = "prop_warranty_expiry", Name = "Warranty Expiration", DataType = "date", Required = false, DisplayOrder = 5 },
                ],
                CreatedBy = "system"
            },
            new()
            {
                Id = "type_phone",
                Name = "IP Phone",
                Description = "VoIP telephone",
                Properties =
                [
                    new AssetTypeProperty { PropertyId = "prop_voip_support", Name = "VoIP Support", DataType = "boolean", Required = true, DisplayOrder = 1 },
                    new AssetTypeProperty { PropertyId = "prop_poe_class", Name = "PoE Class", DataType = "enum", Required = false, DisplayOrder = 2 },
                    new AssetTypeProperty { PropertyId = "prop_ip_address", Name = "IP Address", DataType = "string", Required = false, DisplayOrder = 3 },
                    new AssetTypeProperty { PropertyId = "prop_mac_address", Name = "MAC Address", DataType = "string", Required = false, DisplayOrder = 4 },
                    new AssetTypeProperty { PropertyId = "prop_warranty_expiry", Name = "Warranty Expiration", DataType = "date", Required = false, DisplayOrder = 5 },
                ],
                CreatedBy = "system"
            },
            new()
            {
                Id = "type_furniture",
                Name = "Furniture",
                Description = "Office furniture",
                Properties =
                [
                    new AssetTypeProperty { PropertyId = "prop_dimensions", Name = "Dimensions", DataType = "string", Required = false, DisplayOrder = 1 },
                    new AssetTypeProperty { PropertyId = "prop_material", Name = "Material", DataType = "enum", Required = false, DisplayOrder = 2 },
                    new AssetTypeProperty { PropertyId = "prop_color", Name = "Color", DataType = "string", Required = false, DisplayOrder = 3 },
                ],
                CreatedBy = "system"
            },
            new()
            {
                Id = "type_desk",
                Name = "Desk",
                Description = "Office desk",
                ParentTypeId = "type_furniture",
                Properties =
                [
                    new AssetTypeProperty { PropertyId = "prop_adjustable", Name = "Adjustable", DataType = "boolean", Required = false, DisplayOrder = 4 },
                ],
                CreatedBy = "system"
            },
            new()
            {
                Id = "type_chair",
                Name = "Office Chair",
                Description = "Office seating",
                ParentTypeId = "type_furniture",
                Properties =
                [
                    new AssetTypeProperty { PropertyId = "prop_adjustable", Name = "Adjustable", DataType = "boolean", Required = false, DisplayOrder = 4 },
                ],
                CreatedBy = "system"
            },
        };

        await _assetTypes.InsertManyAsync(assetTypes);
        logger.LogInformation("Seeded {Count} asset types.", assetTypes.Count);
    }

    private async Task SeedAssetDefinitionsAsync()
    {
        var count = await _assetDefinitions.CountDocumentsAsync(FilterDefinition<AssetDefinition>.Empty);
        if (count > 0)
        {
            logger.LogInformation("Asset definitions already seeded. Skipping.");
            return;
        }

        logger.LogInformation("Seeding asset definitions...");

        var defaultWarrantyEnd = DateTime.UtcNow.AddYears(3).Date;

        var definitions = new List<AssetDefinition>
        {
            // Laptops
            new()
            {
                Id = "def_dell_latitude_5430",
                Name = "Dell Latitude 5430",
                ShortName = "LAT5430",
                AssetTypeId = "type_laptop",
                AssetTypeName = "Laptop",
                Manufacturer = "Dell",
                Model = "Latitude 5430",
                Description = "14-inch business laptop with Intel processor",
                Specifications =
                [
                    new() { PropertyId = "prop_cpu", Name = "CPU", Value = "Intel Core i5-1245U", DataType = "string" },
                    new() { PropertyId = "prop_ram", Name = "RAM", Value = 16, DataType = "number", Unit = "GB" },
                    new() { PropertyId = "prop_storage", Name = "Storage", Value = 512, DataType = "number", Unit = "GB" },
                    new() { PropertyId = "prop_screen_size", Name = "Screen Size", Value = 14, DataType = "number", Unit = "inches" },
                    new() { PropertyId = "prop_resolution", Name = "Resolution", Value = "1920x1080", DataType = "string" },
                    new() { PropertyId = "prop_warranty_expiry", Name = "Warranty Expiration", Value = defaultWarrantyEnd, DataType = "date" },
                ],
                Tags = ["Business", "Mobile", "Intel"],
                CreatedBy = "system"
            },
            new()
            {
                Id = "def_macbook_pro_14",
                Name = "Apple MacBook Pro 14-inch",
                ShortName = "MBP14",
                AssetTypeId = "type_laptop",
                AssetTypeName = "Laptop",
                Manufacturer = "Apple",
                Model = "MacBook Pro 14-inch (M3)",
                Description = "14-inch MacBook Pro with M3 chip",
                Specifications =
                [
                    new() { PropertyId = "prop_cpu", Name = "CPU", Value = "Apple M3", DataType = "string" },
                    new() { PropertyId = "prop_ram", Name = "RAM", Value = 16, DataType = "number", Unit = "GB" },
                    new() { PropertyId = "prop_storage", Name = "Storage", Value = 512, DataType = "number", Unit = "GB" },
                    new() { PropertyId = "prop_screen_size", Name = "Screen Size", Value = 14, DataType = "number", Unit = "inches" },
                    new() { PropertyId = "prop_resolution", Name = "Resolution", Value = "3024x1964", DataType = "string" },
                    new() { PropertyId = "prop_warranty_expiry", Name = "Warranty Expiration", Value = defaultWarrantyEnd, DataType = "date" },
                ],
                Tags = ["Apple", "Mobile", "Development"],
                CreatedBy = "system"
            },
            new()
            {
                Id = "def_lenovo_thinkpad_x1",
                Name = "Lenovo ThinkPad X1 Carbon Gen 11",
                ShortName = "X1C11",
                AssetTypeId = "type_laptop",
                AssetTypeName = "Laptop",
                Manufacturer = "Lenovo",
                Model = "ThinkPad X1 Carbon Gen 11",
                Description = "Ultra-light business laptop",
                Specifications =
                [
                    new() { PropertyId = "prop_cpu", Name = "CPU", Value = "Intel Core i7-1365U", DataType = "string" },
                    new() { PropertyId = "prop_ram", Name = "RAM", Value = 32, DataType = "number", Unit = "GB" },
                    new() { PropertyId = "prop_storage", Name = "Storage", Value = 1024, DataType = "number", Unit = "GB" },
                    new() { PropertyId = "prop_screen_size", Name = "Screen Size", Value = 14, DataType = "number", Unit = "inches" },
                    new() { PropertyId = "prop_resolution", Name = "Resolution", Value = "2880x1800", DataType = "string" },
                    new() { PropertyId = "prop_warranty_expiry", Name = "Warranty Expiration", Value = defaultWarrantyEnd, DataType = "date" },
                ],
                Tags = ["Business", "Premium", "Intel"],
                CreatedBy = "system"
            },

            // Desktops
            new()
            {
                Id = "def_dell_optiplex_7090",
                Name = "Dell OptiPlex 7090",
                ShortName = "OPT7090",
                AssetTypeId = "type_desktop",
                AssetTypeName = "Desktop",
                Manufacturer = "Dell",
                Model = "OptiPlex 7090",
                Description = "Business desktop workstation",
                Specifications =
                [
                    new() { PropertyId = "prop_cpu", Name = "CPU", Value = "Intel Core i7-11700", DataType = "string" },
                    new() { PropertyId = "prop_ram", Name = "RAM", Value = 16, DataType = "number", Unit = "GB" },
                    new() { PropertyId = "prop_storage", Name = "Storage", Value = 512, DataType = "number", Unit = "GB" },
                    new() { PropertyId = "prop_warranty_expiry", Name = "Warranty Expiration", Value = defaultWarrantyEnd, DataType = "date" },
                ],
                Tags = ["Business", "Desktop", "Intel"],
                CreatedBy = "system"
            },

            // Monitors
            new()
            {
                Id = "def_dell_p2722h",
                Name = "Dell P2722H",
                ShortName = "P2722H",
                AssetTypeId = "type_monitor",
                AssetTypeName = "Monitor",
                Manufacturer = "Dell",
                Model = "P2722H",
                Description = "27-inch Full HD monitor",
                Specifications =
                [
                    new() { PropertyId = "prop_screen_size", Name = "Screen Size", Value = 27, DataType = "number", Unit = "inches" },
                    new() { PropertyId = "prop_resolution", Name = "Resolution", Value = "1920x1080", DataType = "string" },
                    new() { PropertyId = "prop_panel_type", Name = "Panel Type", Value = "IPS", DataType = "string" },
                    new() { PropertyId = "prop_refresh_rate", Name = "Refresh Rate", Value = 60, DataType = "number", Unit = "Hz" },
                    new() { PropertyId = "prop_warranty_expiry", Name = "Warranty Expiration", Value = defaultWarrantyEnd, DataType = "date" },
                ],
                Tags = ["Display", "Office"],
                CreatedBy = "system"
            },
            new()
            {
                Id = "def_lg_27up850",
                Name = "LG 27UP850",
                ShortName = "27UP850",
                AssetTypeId = "type_monitor",
                AssetTypeName = "Monitor",
                Manufacturer = "LG",
                Model = "27UP850",
                Description = "27-inch 4K UHD monitor",
                Specifications =
                [
                    new() { PropertyId = "prop_screen_size", Name = "Screen Size", Value = 27, DataType = "number", Unit = "inches" },
                    new() { PropertyId = "prop_resolution", Name = "Resolution", Value = "3840x2160", DataType = "string" },
                    new() { PropertyId = "prop_panel_type", Name = "Panel Type", Value = "IPS", DataType = "string" },
                    new() { PropertyId = "prop_refresh_rate", Name = "Refresh Rate", Value = 60, DataType = "number", Unit = "Hz" },
                    new() { PropertyId = "prop_warranty_expiry", Name = "Warranty Expiration", Value = defaultWarrantyEnd, DataType = "date" },
                ],
                Tags = ["Display", "4K", "Premium"],
                CreatedBy = "system"
            },

            // Printers
            new()
            {
                Id = "def_hp_laserjet_pro",
                Name = "HP LaserJet Pro M404dn",
                ShortName = "LJM404",
                AssetTypeId = "type_printer",
                AssetTypeName = "Printer",
                Manufacturer = "HP",
                Model = "LaserJet Pro M404dn",
                Description = "Monochrome laser printer",
                Specifications =
                [
                    new() { PropertyId = "prop_print_technology", Name = "Print Technology", Value = "Laser", DataType = "string" },
                    new() { PropertyId = "prop_color_support", Name = "Color Support", Value = false, DataType = "boolean" },
                    new() { PropertyId = "prop_duplex", Name = "Duplex Printing", Value = true, DataType = "boolean" },
                    new() { PropertyId = "prop_print_speed", Name = "Print Speed", Value = 38, DataType = "number", Unit = "ppm" },
                    new() { PropertyId = "prop_warranty_expiry", Name = "Warranty Expiration", Value = defaultWarrantyEnd, DataType = "date" },
                ],
                Tags = ["Printer", "Laser", "Monochrome"],
                CreatedBy = "system"
            },
            new()
            {
                Id = "def_canon_pixma",
                Name = "Canon PIXMA TR8620",
                ShortName = "TR8620",
                AssetTypeId = "type_printer",
                AssetTypeName = "Printer",
                Manufacturer = "Canon",
                Model = "PIXMA TR8620",
                Description = "All-in-one color inkjet printer",
                Specifications =
                [
                    new() { PropertyId = "prop_print_technology", Name = "Print Technology", Value = "Inkjet", DataType = "string" },
                    new() { PropertyId = "prop_color_support", Name = "Color Support", Value = true, DataType = "boolean" },
                    new() { PropertyId = "prop_duplex", Name = "Duplex Printing", Value = true, DataType = "boolean" },
                    new() { PropertyId = "prop_print_speed", Name = "Print Speed", Value = 15, DataType = "number", Unit = "ppm" },
                    new() { PropertyId = "prop_warranty_expiry", Name = "Warranty Expiration", Value = defaultWarrantyEnd, DataType = "date" },
                ],
                Tags = ["Printer", "Inkjet", "Color", "All-in-One"],
                CreatedBy = "system"
            },

            // Network Equipment
            new()
            {
                Id = "def_cisco_catalyst",
                Name = "Cisco Catalyst 2960-X",
                ShortName = "C2960X",
                AssetTypeId = "type_network_switch",
                AssetTypeName = "Network Switch",
                Manufacturer = "Cisco",
                Model = "Catalyst 2960-X",
                Description = "24-port Gigabit managed switch",
                Specifications =
                [
                    new() { PropertyId = "prop_port_count", Name = "Port Count", Value = 24, DataType = "number" },
                    new() { PropertyId = "prop_speed", Name = "Speed", Value = "1 Gbps", DataType = "string" },
                    new() { PropertyId = "prop_poe_support", Name = "PoE Support", Value = true, DataType = "boolean" },
                    new() { PropertyId = "prop_warranty_expiry", Name = "Warranty Expiration", Value = defaultWarrantyEnd, DataType = "date" },
                ],
                Tags = ["Network", "Switch", "Managed", "PoE"],
                CreatedBy = "system"
            },

            // IP Phones
            new()
            {
                Id = "def_yealink_t46s",
                Name = "Yealink SIP-T46S",
                ShortName = "T46S",
                AssetTypeId = "type_phone",
                AssetTypeName = "IP Phone",
                Manufacturer = "Yealink",
                Model = "SIP-T46S",
                Description = "VoIP desk phone",
                Specifications =
                [
                    new() { PropertyId = "prop_voip_support", Name = "VoIP Support", Value = true, DataType = "boolean" },
                    new() { PropertyId = "prop_poe_class", Name = "PoE Class", Value = "Class 2", DataType = "string" },
                    new() { PropertyId = "prop_warranty_expiry", Name = "Warranty Expiration", Value = defaultWarrantyEnd, DataType = "date" },
                ],
                Tags = ["Phone", "VoIP", "PoE"],
                CreatedBy = "system"
            },

            // Furniture
            new()
            {
                Id = "def_steelcase_series1",
                Name = "Steelcase Series 1",
                ShortName = "SC-S1",
                AssetTypeId = "type_chair",
                AssetTypeName = "Office Chair",
                Manufacturer = "Steelcase",
                Model = "Series 1",
                Description = "Ergonomic office chair",
                Specifications =
                [
                    new() { PropertyId = "prop_material", Name = "Material", Value = "Composite", DataType = "string" },
                    new() { PropertyId = "prop_color", Name = "Color", Value = "Black", DataType = "string" },
                    new() { PropertyId = "prop_adjustable", Name = "Adjustable", Value = true, DataType = "boolean" },
                ],
                Tags = ["Furniture", "Seating", "Ergonomic"],
                CreatedBy = "system"
            },
            new()
            {
                Id = "def_jarvis_desk",
                Name = "Fully Jarvis Standing Desk",
                ShortName = "JARVIS",
                AssetTypeId = "type_desk",
                AssetTypeName = "Desk",
                Manufacturer = "Fully",
                Model = "Jarvis",
                Description = "Electric standing desk",
                Specifications =
                [
                    new() { PropertyId = "prop_dimensions", Name = "Dimensions", Value = "60x30 inches", DataType = "string" },
                    new() { PropertyId = "prop_material", Name = "Material", Value = "Wood", DataType = "string" },
                    new() { PropertyId = "prop_color", Name = "Color", Value = "Walnut", DataType = "string" },
                    new() { PropertyId = "prop_adjustable", Name = "Adjustable", Value = true, DataType = "boolean" },
                ],
                Tags = ["Furniture", "Desk", "Standing", "Electric"],
                CreatedBy = "system"
            },
        };

        await _assetDefinitions.InsertManyAsync(definitions);
        logger.LogInformation("Seeded {Count} asset definitions.", definitions.Count);
    }

    private async Task SeedAssetsAsync()
    {
        var count = await _assets.CountDocumentsAsync(FilterDefinition<Asset>.Empty);
        if (count > 0)
        {
            logger.LogInformation("Assets already seeded. Skipping.");
            return;
        }

        logger.LogInformation("Seeding sample assets...");

        var now = DateTime.UtcNow;
        var assets = new List<Asset>
        {
            // Laptops
            new()
            {
                SerialNumber = "DL5430-2024-001",
                AssetTag = "LAP-001",
                Status = "active",
                Definition = new AssetDefinitionSnapshot
                {
                    DefinitionId = "def_dell_latitude_5430",
                    Name = "Dell Latitude 5430",
                    AssetTypeId = "type_laptop",
                    AssetTypeName = "Laptop",
                    Manufacturer = "Dell",
                    Model = "Latitude 5430",
                    Specifications =
                    [
                        new() { PropertyId = "prop_cpu", Name = "CPU", Value = "Intel Core i5-1245U", DataType = "string" },
                        new() { PropertyId = "prop_ram", Name = "RAM", Value = 16, DataType = "number", Unit = "GB" },
                        new() { PropertyId = "prop_storage", Name = "Storage", Value = 512, DataType = "number", Unit = "GB" },
                        new() { PropertyId = "prop_os", Name = "Operating System", Value = "Windows 11", DataType = "string" },
                        new() { PropertyId = "prop_hostname", Name = "Hostname", Value = "LAP-001-WIN11", DataType = "string" },
                        new() { PropertyId = "prop_warranty_expiry", Name = "Warranty Expiration", Value = now.AddYears(3).AddMonths(-6).Date, DataType = "date" },
                    ]
                },
                PurchaseInfo = new PurchaseInfo
                {
                    PurchaseDate = now.AddMonths(-6),
                    PurchasePrice = 1299.99m,
                    Currency = "USD",
                    Vendor = "Dell Direct",
                    WarrantyExpiry = now.AddYears(3).AddMonths(-6)
                },
                LocationId = "room_hq_main_101",
                Location = new AssetLocation { Building = "Main Office", Room = "Room 101", Desk = "Desk A" },
                Assignment = new AssetAssignment
                {
                    AssignedToUserId = "70025a13-16e3-45cb-ae4c-5cf95b90a625",
                    AssignedToName = "System Administrator",
                    AssignedAt = now.AddMonths(-5),
                    AssignmentType = "permanent"
                },
                CreatedBy = "system"
            },
            new()
            {
                SerialNumber = "MBP14-2024-001",
                AssetTag = "LAP-002",
                Status = "active",
                Definition = new AssetDefinitionSnapshot
                {
                    DefinitionId = "def_macbook_pro_14",
                    Name = "Apple MacBook Pro 14-inch",
                    AssetTypeId = "type_laptop",
                    AssetTypeName = "Laptop",
                    Manufacturer = "Apple",
                    Model = "MacBook Pro 14-inch (M3)",
                    Specifications =
                    [
                        new() { PropertyId = "prop_cpu", Name = "CPU", Value = "Apple M3", DataType = "string" },
                        new() { PropertyId = "prop_ram", Name = "RAM", Value = 16, DataType = "number", Unit = "GB" },
                        new() { PropertyId = "prop_storage", Name = "Storage", Value = 512, DataType = "number", Unit = "GB" },
                        new() { PropertyId = "prop_os", Name = "Operating System", Value = "macOS Sonoma", DataType = "string" },
                        new() { PropertyId = "prop_hostname", Name = "Hostname", Value = "LAP-002-MACOS", DataType = "string" },
                        new() { PropertyId = "prop_warranty_expiry", Name = "Warranty Expiration", Value = now.AddYears(1).AddMonths(-3).Date, DataType = "date" },
                    ]
                },
                PurchaseInfo = new PurchaseInfo
                {
                    PurchaseDate = now.AddMonths(-3),
                    PurchasePrice = 2499.00m,
                    Currency = "USD",
                    Vendor = "Apple Store",
                    WarrantyExpiry = now.AddYears(1).AddMonths(-3)
                },
                LocationId = "b2c3d4e5-f6a7-4b5c-9d0e-1f2a3b4c5d6e", // Room 102
                Location = new AssetLocation { Building = "Main Office", Room = "Room 102", Desk = "Desk B" },
                CreatedBy = "system"
            },
            
            // Monitors
            new()
            {
                SerialNumber = "P2722H-2024-001",
                AssetTag = "MON-001",
                Status = "active",
                Definition = new AssetDefinitionSnapshot
                {
                    DefinitionId = "def_dell_p2722h",
                    Name = "Dell P2722H",
                    AssetTypeId = "type_monitor",
                    AssetTypeName = "Monitor",
                    Manufacturer = "Dell",
                    Model = "P2722H",
                    Specifications =
                    [
                        new() { PropertyId = "prop_screen_size", Name = "Screen Size", Value = 27, DataType = "number", Unit = "inches" },
                        new() { PropertyId = "prop_resolution", Name = "Resolution", Value = "1920x1080", DataType = "string" },
                        new() { PropertyId = "prop_warranty_expiry", Name = "Warranty Expiration", Value = now.AddYears(2).Date, DataType = "date" },
                    ]
                },
                PurchaseInfo = new PurchaseInfo
                {
                    PurchaseDate = now.AddYears(-1),
                    PurchasePrice = 299.99m,
                    Currency = "USD",
                    Vendor = "Dell Direct"
                },                LocationId = "a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d", // Room 101                Location = new AssetLocation { Building = "Main Office", Room = "Room 101", Desk = "Desk A" },
                Assignment = new AssetAssignment
                {
                    AssignedToUserId = "70025a13-16e3-45cb-ae4c-5cf95b90a625",
                    AssignedToName = "System Administrator",
                    AssignedAt = now.AddMonths(-11),
                    AssignmentType = "permanent"
                },
                CreatedBy = "system"
            },
            new()
            {
                SerialNumber = "P2722H-2024-002",
                AssetTag = "MON-002",
                Status = "active",
                Definition = new AssetDefinitionSnapshot
                {
                    DefinitionId = "def_dell_p2722h",
                    Name = "Dell P2722H",
                    AssetTypeId = "type_monitor",
                    AssetTypeName = "Monitor",
                    Manufacturer = "Dell",
                    Model = "P2722H",
                    Specifications =
                    [
                        new() { PropertyId = "prop_screen_size", Name = "Screen Size", Value = 27, DataType = "number", Unit = "inches" },
                        new() { PropertyId = "prop_resolution", Name = "Resolution", Value = "1920x1080", DataType = "string" },
                        new() { PropertyId = "prop_warranty_expiry", Name = "Warranty Expiration", Value = now.AddYears(2).Date, DataType = "date" },
                    ]
                },
                PurchaseInfo = new PurchaseInfo
                {
                    PurchaseDate = now.AddYears(-1),
                    PurchasePrice = 299.99m,
                    Currency = "USD",
                    Vendor = "Dell Direct"
                },
                LocationId = "c3d4e5f6-a7b8-4c5d-0e1f-2a3b4c5d6e7f", // Room 201
                Location = new AssetLocation { Building = "Main Office", Room = "Room 101", Desk = "Desk A" },
                Assignment = new AssetAssignment
                {
                    AssignedToUserId = "70025a13-16e3-45cb-ae4c-5cf95b90a625",
                    AssignedToName = "System Administrator",
                    AssignedAt = now.AddMonths(-11),
                    AssignmentType = "permanent"
                },
                CreatedBy = "system"
            },
            
            // Printer
            new()
            {
                SerialNumber = "LJM404-2024-001",
                AssetTag = "PRN-001",
                Status = "active",
                Definition = new AssetDefinitionSnapshot
                {
                    DefinitionId = "def_hp_laserjet_pro",
                    Name = "HP LaserJet Pro M404dn",
                    AssetTypeId = "type_printer",
                    AssetTypeName = "Printer",
                    Manufacturer = "HP",
                    Model = "LaserJet Pro M404dn",
                    Specifications =
                    [
                        new() { PropertyId = "prop_print_technology", Name = "Print Technology", Value = "Laser", DataType = "string" },
                        new() { PropertyId = "prop_color_support", Name = "Color Support", Value = false, DataType = "boolean" },
                        new() { PropertyId = "prop_ip_address", Name = "IP Address", Value = "192.168.1.50", DataType = "string" },
                        new() { PropertyId = "prop_warranty_expiry", Name = "Warranty Expiration", Value = now.AddMonths(-12).Date, DataType = "date" },
                    ]
                },
                PurchaseInfo = new PurchaseInfo
                {
                    PurchaseDate = now.AddYears(-2),
                    PurchasePrice = 349.99m,
                    Currency = "USD",
                    Vendor = "HP Direct",
                    WarrantyExpiry = now.AddMonths(-12)
                },
                LocationId = "e5f6a7b8-c9d0-4e5f-2a3b-4c5d6e7f8a9b", // Workshop
                Location = new AssetLocation { Building = "Main Office", Room = "Print Room" },
                CreatedBy = "system"
            },
            
            // Network Switch
            new()
            {
                SerialNumber = "C2960X-2024-001",
                AssetTag = "NET-001",
                Status = "active",
                Definition = new AssetDefinitionSnapshot
                {
                    DefinitionId = "def_cisco_catalyst",
                    Name = "Cisco Catalyst 2960-X",
                    AssetTypeId = "type_network_switch",
                    AssetTypeName = "Network Switch",
                    Manufacturer = "Cisco",
                    Model = "Catalyst 2960-X",
                    Specifications =
                    [
                        new() { PropertyId = "prop_port_count", Name = "Port Count", Value = 24, DataType = "number" },
                        new() { PropertyId = "prop_speed", Name = "Speed", Value = "1 Gbps", DataType = "string" },
                        new() { PropertyId = "prop_ip_address", Name = "IP Address", Value = "192.168.1.1", DataType = "string" },
                        new() { PropertyId = "prop_warranty_expiry", Name = "Warranty Expiration", Value = now.AddMonths(-12).Date, DataType = "date" },
                    ]
                },
                PurchaseInfo = new PurchaseInfo
                {
                    PurchaseDate = now.AddYears(-3),
                    PurchasePrice = 899.99m,
                    Currency = "USD",
                    Vendor = "Cisco Reseller",
                    WarrantyExpiry = now.AddMonths(-12)
                },
                LocationId = "d4e5f6a7-b8c9-4d5e-1f2a-3b4c5d6e7f8a", // Server Room
                Location = new AssetLocation { Building = "Main Office", Room = "Server Room" },
                CreatedBy = "system"
            },
            
            // IP Phones
            new()
            {
                SerialNumber = "T46S-2024-001",
                AssetTag = "PHN-001",
                Status = "active",
                Definition = new AssetDefinitionSnapshot
                {
                    DefinitionId = "def_yealink_t46s",
                    Name = "Yealink SIP-T46S",
                    AssetTypeId = "type_phone",
                    AssetTypeName = "IP Phone",
                    Manufacturer = "Yealink",
                    Model = "SIP-T46S",
                    Specifications =
                    [
                        new() { PropertyId = "prop_voip_support", Name = "VoIP Support", Value = true, DataType = "boolean" },
                        new() { PropertyId = "prop_ip_address", Name = "IP Address", Value = "192.168.1.101", DataType = "string" },
                        new() { PropertyId = "prop_warranty_expiry", Name = "Warranty Expiration", Value = now.AddYears(1).Date, DataType = "date" },
                    ]
                },
                PurchaseInfo = new PurchaseInfo
                {
                    PurchaseDate = now.AddMonths(-8),
                    PurchasePrice = 159.99m,
                    Currency = "USD",
                    Vendor = "VoIP Supplier"
                },
                LocationId = "a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d", // Room 101
                Location = new AssetLocation { Building = "Main Office", Room = "Room 101", Desk = "Desk A" },
                Assignment = new AssetAssignment
                {
                    AssignedToUserId = "70025a13-16e3-45cb-ae4c-5cf95b90a625",
                    AssignedToName = "System Administrator",
                    AssignedAt = now.AddMonths(-8),
                    AssignmentType = "permanent"
                },
                CreatedBy = "system"
            },
            
            // Furniture
            new()
            {
                SerialNumber = "SC-S1-2024-001",
                AssetTag = "CHR-001",
                Status = "active",
                Definition = new AssetDefinitionSnapshot
                {
                    DefinitionId = "def_steelcase_series1",
                    Name = "Steelcase Series 1",
                    AssetTypeId = "type_chair",
                    AssetTypeName = "Office Chair",
                    Manufacturer = "Steelcase",
                    Model = "Series 1",
                    Specifications =
                    [
                        new() { PropertyId = "prop_material", Name = "Material", Value = "Composite", DataType = "string" },
                        new() { PropertyId = "prop_color", Name = "Color", Value = "Black", DataType = "string" },
                    ]
                },
                PurchaseInfo = new PurchaseInfo
                {
                    PurchaseDate = now.AddYears(-1),
                    PurchasePrice = 549.99m,
                    Currency = "USD",
                    Vendor = "Office Furniture Inc"
                },
                LocationId = "a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d", // Room 101
                Location = new AssetLocation { Building = "Main Office", Room = "Room 101", Desk = "Desk A" },
                Assignment = new AssetAssignment
                {
                    AssignedToUserId = "70025a13-16e3-45cb-ae4c-5cf95b90a625",
                    AssignedToName = "System Administrator",
                    AssignedAt = now.AddMonths(-11),
                    AssignmentType = "permanent"
                },
                CreatedBy = "system"
            },
            
            // Additional assets to cover all rooms
            // Room 201 - Desktop PC
            new()
            {
                SerialNumber = "PC-OPT-001",
                AssetTag = "PC-001",
                Status = "active",
                Definition = new AssetDefinitionSnapshot
                {
                    DefinitionId = "def_dell_optiplex",
                    Name = "Dell OptiPlex 7090",
                    AssetTypeId = "type_desktop",
                    AssetTypeName = "Desktop PC",
                    Manufacturer = "Dell",
                    Model = "OptiPlex 7090",
                    Specifications =
                    [
                        new() { PropertyId = "prop_cpu", Name = "CPU", Value = "Intel Core i7-11700", DataType = "string" },
                        new() { PropertyId = "prop_ram", Name = "RAM", Value = 32, DataType = "number", Unit = "GB" },
                        new() { PropertyId = "prop_storage", Name = "Storage", Value = 1024, DataType = "number", Unit = "GB" },
                    ]
                },
                PurchaseInfo = new PurchaseInfo
                {
                    PurchaseDate = now.AddMonths(-8),
                    PurchasePrice = 1499.99m,
                    Currency = "USD",
                    Vendor = "Dell Direct"
                },
                LocationId = "c3d4e5f6-a7b8-4c5d-0e1f-2a3b4c5d6e7f", // Room 201
                Location = new AssetLocation { Building = "Main Office", Room = "Room 201" },
                CreatedBy = "system"
            },
            // Server Room - Rack Server
            new()
            {
                SerialNumber = "R740-2024-001",
                AssetTag = "SRV-001",
                Status = "active",
                Definition = new AssetDefinitionSnapshot
                {
                    DefinitionId = "def_dell_poweredge",
                    Name = "Dell PowerEdge R740",
                    AssetTypeId = "type_server",
                    AssetTypeName = "Server",
                    Manufacturer = "Dell",
                    Model = "PowerEdge R740",
                    Specifications =
                    [
                        new() { PropertyId = "prop_cpu", Name = "CPU", Value = "Dual Intel Xeon Gold 6226R", DataType = "string" },
                        new() { PropertyId = "prop_ram", Name = "RAM", Value = 256, DataType = "number", Unit = "GB" },
                    ]
                },
                PurchaseInfo = new PurchaseInfo
                {
                    PurchaseDate = now.AddYears(-2),
                    PurchasePrice = 8999.99m,
                    Currency = "USD",
                    Vendor = "Dell Enterprise"
                },
                LocationId = "d4e5f6a7-b8c9-4d5e-1f2a-3b4c5d6e7f8a", // Server Room
                Location = new AssetLocation { Building = "Main Office", Room = "Server Room" },
                CreatedBy = "system"
            },
            // Equipment Storage - Laptop
            new()
            {
                SerialNumber = "LAT-5440-001",
                AssetTag = "LAP-003",
                Status = "active",
                Definition = new AssetDefinitionSnapshot
                {
                    DefinitionId = "def_dell_latitude",
                    Name = "Dell Latitude 5440",
                    AssetTypeId = "type_laptop",
                    AssetTypeName = "Laptop",
                    Manufacturer = "Dell",
                    Model = "Latitude 5440",
                    Specifications =
                    [
                        new() { PropertyId = "prop_cpu", Name = "CPU", Value = "Intel Core i7-1355U", DataType = "string" },
                        new() { PropertyId = "prop_ram", Name = "RAM", Value = 16, DataType = "number", Unit = "GB" },
                    ]
                },
                PurchaseInfo = new PurchaseInfo
                {
                    PurchaseDate = now.AddMonths(-4),
                    PurchasePrice = 1599.99m,
                    Currency = "USD",
                    Vendor = "Dell Direct"
                },
                LocationId = "f6a7b8c9-d0e1-4f5a-3b4c-5d6e7f8a9b0c", // Equipment Storage
                Location = new AssetLocation { Building = "Annex", Room = "Storage" },
                CreatedBy = "system"
            },
            // Server Room A - Network Switch
            new()
            {
                SerialNumber = "C9300-001",
                AssetTag = "NET-002",
                Status = "active",
                Definition = new AssetDefinitionSnapshot
                {
                    DefinitionId = "def_cisco_catalyst",
                    Name = "Cisco Catalyst 9300",
                    AssetTypeId = "type_network_switch",
                    AssetTypeName = "Network Switch",
                    Manufacturer = "Cisco",
                    Model = "Catalyst 9300",
                    Specifications =
                    [
                        new() { PropertyId = "prop_port_count", Name = "Port Count", Value = 48, DataType = "number" },
                    ]
                },
                PurchaseInfo = new PurchaseInfo
                {
                    PurchaseDate = now.AddYears(-1),
                    PurchasePrice = 5999.99m,
                    Currency = "USD",
                    Vendor = "Cisco Direct"
                },
                LocationId = "a7b8c9d0-e1f2-4a5b-4c5d-6e7f8a9b0c1d", // Server Room A
                Location = new AssetLocation { Building = "Data Center", Room = "Server Room A" },
                CreatedBy = "system"
            },
            // Server Room B - Server
            new()
            {
                SerialNumber = "R750-2024-001",
                AssetTag = "SRV-002",
                Status = "active",
                Definition = new AssetDefinitionSnapshot
                {
                    DefinitionId = "def_dell_poweredge",
                    Name = "Dell PowerEdge R750",
                    AssetTypeId = "type_server",
                    AssetTypeName = "Server",
                    Manufacturer = "Dell",
                    Model = "PowerEdge R750",
                    Specifications =
                    [
                        new() { PropertyId = "prop_cpu", Name = "CPU", Value = "Dual Intel Xeon Gold 6338", DataType = "string" },
                        new() { PropertyId = "prop_ram", Name = "RAM", Value = 512, DataType = "number", Unit = "GB" },
                    ]
                },
                PurchaseInfo = new PurchaseInfo
                {
                    PurchaseDate = now.AddYears(-1),
                    PurchasePrice = 12999.99m,
                    Currency = "USD",
                    Vendor = "Dell Enterprise"
                },
                LocationId = "b8c9d0e1-f2a3-4b5c-5d6e-7f8a9b0c1d2e", // Server Room B
                Location = new AssetLocation { Building = "Data Center", Room = "Server Room B" },
                CreatedBy = "system"
            },
            // Network Operations Center - Desktop PC
            new()
            {
                SerialNumber = "PC-OPT-002",
                AssetTag = "PC-002",
                Status = "active",
                Definition = new AssetDefinitionSnapshot
                {
                    DefinitionId = "def_dell_optiplex",
                    Name = "Dell OptiPlex 7090",
                    AssetTypeId = "type_desktop",
                    AssetTypeName = "Desktop PC",
                    Manufacturer = "Dell",
                    Model = "OptiPlex 7090",
                    Specifications =
                    [
                        new() { PropertyId = "prop_cpu", Name = "CPU", Value = "Intel Core i7-11700", DataType = "string" },
                        new() { PropertyId = "prop_ram", Name = "RAM", Value = 32, DataType = "number", Unit = "GB" },
                    ]
                },
                PurchaseInfo = new PurchaseInfo
                {
                    PurchaseDate = now.AddMonths(-10),
                    PurchasePrice = 1499.99m,
                    Currency = "USD",
                    Vendor = "Dell Direct"
                },
                LocationId = "c9d0e1f2-a3b4-4c5d-6e7f-8a9b0c1d2e3f", // Network Operations Center
                Location = new AssetLocation { Building = "Data Center", Room = "NOC" },
                CreatedBy = "system"
            },
            // Open Office Space - Multiple Laptops
            new()
            {
                SerialNumber = "LAT-5450-001",
                AssetTag = "LAP-004",
                Status = "active",
                Definition = new AssetDefinitionSnapshot
                {
                    DefinitionId = "def_dell_latitude",
                    Name = "Dell Latitude 5450",
                    AssetTypeId = "type_laptop",
                    AssetTypeName = "Laptop",
                    Manufacturer = "Dell",
                    Model = "Latitude 5450",
                    Specifications =
                    [
                        new() { PropertyId = "prop_cpu", Name = "CPU", Value = "Intel Core i5-1345U", DataType = "string" },
                        new() { PropertyId = "prop_ram", Name = "RAM", Value = 16, DataType = "number", Unit = "GB" },
                    ]
                },
                PurchaseInfo = new PurchaseInfo
                {
                    PurchaseDate = now.AddMonths(-5),
                    PurchasePrice = 1399.99m,
                    Currency = "USD",
                    Vendor = "Dell Direct"
                },
                LocationId = "d0e1f2a3-b4c5-4d5e-7f8a-9b0c1d2e3f4a", // Open Office Space
                Location = new AssetLocation { Building = "LA Office", Room = "Open Space" },
                CreatedBy = "system"
            },
            new()
            {
                SerialNumber = "LAT-5450-002",
                AssetTag = "LAP-005",
                Status = "active",
                Definition = new AssetDefinitionSnapshot
                {
                    DefinitionId = "def_dell_latitude",
                    Name = "Dell Latitude 5450",
                    AssetTypeId = "type_laptop",
                    AssetTypeName = "Laptop",
                    Manufacturer = "Dell",
                    Model = "Latitude 5450",
                    Specifications =
                    [
                        new() { PropertyId = "prop_cpu", Name = "CPU", Value = "Intel Core i5-1345U", DataType = "string" },
                        new() { PropertyId = "prop_ram", Name = "RAM", Value = 16, DataType = "number", Unit = "GB" },
                    ]
                },
                PurchaseInfo = new PurchaseInfo
                {
                    PurchaseDate = now.AddMonths(-5),
                    PurchasePrice = 1399.99m,
                    Currency = "USD",
                    Vendor = "Dell Direct"
                },
                LocationId = "d0e1f2a3-b4c5-4d5e-7f8a-9b0c1d2e3f4a", // Open Office Space
                Location = new AssetLocation { Building = "LA Office", Room = "Open Space" },
                CreatedBy = "system"
            },
            // Conference Room - Monitor and Projector
            new()
            {
                SerialNumber = "P2723DE-001",
                AssetTag = "MON-003",
                Status = "active",
                Definition = new AssetDefinitionSnapshot
                {
                    DefinitionId = "def_dell_p2723de",
                    Name = "Dell P2723DE",
                    AssetTypeId = "type_monitor",
                    AssetTypeName = "Monitor",
                    Manufacturer = "Dell",
                    Model = "P2723DE",
                    Specifications =
                    [
                        new() { PropertyId = "prop_screen_size", Name = "Screen Size", Value = 27, DataType = "number", Unit = "inches" },
                        new() { PropertyId = "prop_resolution", Name = "Resolution", Value = "2560x1440", DataType = "string" },
                    ]
                },
                PurchaseInfo = new PurchaseInfo
                {
                    PurchaseDate = now.AddMonths(-6),
                    PurchasePrice = 449.99m,
                    Currency = "USD",
                    Vendor = "Dell Direct"
                },
                LocationId = "e1f2a3b4-c5d6-4e5f-8a9b-0c1d2e3f4a5b", // Conference Room
                Location = new AssetLocation { Building = "LA Office", Room = "Conference Room" },
                CreatedBy = "system"
            },
            new()
            {
                SerialNumber = "JARVIS-2024-002",
                AssetTag = "DSK-002",
                Status = "active",
                Definition = new AssetDefinitionSnapshot
                {
                    DefinitionId = "def_jarvis_desk",
                    Name = "Fully Jarvis Standing Desk",
                    AssetTypeId = "type_desk",
                    AssetTypeName = "Desk",
                    Manufacturer = "Fully",
                    Model = "Jarvis",
                    Specifications =
                    [
                        new() { PropertyId = "prop_dimensions", Name = "Dimensions", Value = "60x30 inches", DataType = "string" },
                        new() { PropertyId = "prop_material", Name = "Material", Value = "Wood", DataType = "string" },
                    ]
                },
                PurchaseInfo = new PurchaseInfo
                {
                    PurchaseDate = now.AddYears(-1),
                    PurchasePrice = 699.99m,
                    Currency = "USD",
                    Vendor = "Fully Direct"
                },
                LocationId = "e1f2a3b4-c5d6-4e5f-8a9b-0c1d2e3f4a5b", // Conference Room
                Location = new AssetLocation { Building = "LA Office", Room = "Conference Room" },
                CreatedBy = "system"
            },
        };

        await _assets.InsertManyAsync(assets);
        logger.LogInformation("Seeded {Count} sample assets.", assets.Count);
    }
}
