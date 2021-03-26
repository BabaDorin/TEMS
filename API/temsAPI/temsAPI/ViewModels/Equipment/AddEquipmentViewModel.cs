using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.Equipment
{
    public class AddEquipmentViewModel
    {
        public string Id { get; set; }
        public string EquipmentDefinitionID { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; }
        public bool IsDefect { get; set; }
        public bool IsUsed { get; set; }
        public double Price { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string SerialNumber { get; set; }
        public string Temsid { get; set; }
    }
}
