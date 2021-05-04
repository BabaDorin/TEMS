using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using temsAPI.Data.Entities.Report;

namespace temsAPI.Helpers
{

    public partial class ReportHelper
    {

        public static IEquipmentSeparator GetSeparator(ReportTemplate reportTemplate)
        {
            switch (reportTemplate.SepparateBy.ToLower())
            {
                case "type": return new TypeSeparator();
                case "definition": return new DefinitionSeparator();
                case "personnel": return new PersonnelSeparator();
                case "room": return new RoomSeparator();
                default: return new NoneSeparator();
            }
        }

        public enum CommonEquipmentProperties
        {
            Temsid,
            SerialNumber,
            Definition,
            Type,
            Description,
            Price,
            PriceCurrency,
            PurchaseDate,
            Allocatee
        }

        // default association = string
        public static List<Tuple<CommonEquipmentProperties, Type>> CommonPropertyEquipmentTypes = new()
        {
            new Tuple<CommonEquipmentProperties, Type>(CommonEquipmentProperties.Price, typeof(double)),
            new Tuple<CommonEquipmentProperties, Type>(CommonEquipmentProperties.PurchaseDate, typeof(DateTime)),
        };

        public static List<string> CommonProperties = Enum
            .GetValues(typeof(CommonEquipmentProperties))
            .Cast<CommonEquipmentProperties>()
            .Select(q => q.ToString().ToLower())
            .ToList();
    }
}
