using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.Report;
using temsAPI.Helpers.ReportHelpers;
using temsAPI.Helpers.ReportHelpers.CommonPropertyValueProviders;

namespace temsAPI.Helpers
{
    public partial class ReportHelper
    {
        private static PropertyInfo[] equipmentProperties = typeof(Equipment).GetProperties();

        public enum Subjects
        {
            Equipment,
            Personnel,
            Room
        }

        public enum Separators
        {
            None,
            Type,
            Definition,
            Personnel,
            Room
        }

        public enum CommonEquipmentProperties
        {
            Temsid,
            SerialNumber,
            Definition,
            Type,
            Description,
            Price,
            Currency,
            PurchaseDate,
            Allocatee
        }

        public static IEquipmentSeparator GetSeparator(ReportTemplate reportTemplate)
        {
            switch (reportTemplate.SeparateBy.ToLower())
            {
                case "type": return new TypeSeparator();
                case "definition": return new DefinitionSeparator();
                case "personnel": return new PersonnelSeparator();
                case "room": return new RoomSeparator();
                default: return new NoneSeparator();
            }
        }

        public static Type GetCommonPropertyType(string commonProperty)
        {
            switch (commonProperty.ToLower())
            {
                case "price": return typeof(double);
                case "datepurchased": return typeof(DateTime);
                default: return typeof(string);
            }
        }

        public static ICommonPropertyValueProvider GetCommonPropertyValueProvider(string commonPropName, Equipment equipment)
        {
            switch (commonPropName.ToLower())
            {
                case "definition": return new DefinitionValueProvider();
                case "type": return new TypeValueProvider();
                case "allocatee": return new AllocateeValueProvider();
                case "purchasedate": return new DateOnlyValueProvider(commonPropName);
                default: return new DefaultCommonPropertyValueProvider(commonPropName);
            }
        }

        public static List<string> CommonProperties = Enum
            .GetValues(typeof(CommonEquipmentProperties))
            .Cast<CommonEquipmentProperties>()
            .Select(q => q.ToString().ToLower())
            .ToList();
    }
}
