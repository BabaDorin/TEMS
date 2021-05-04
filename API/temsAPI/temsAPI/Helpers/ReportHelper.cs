using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Helpers
{
    public class ReportHelper
    {
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
