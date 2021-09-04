using System;
using temsAPI.Helpers.Filters.Contracts;
using temsAPI.System_Files;

namespace temsAPI.Helpers.EquipmentHelpers
{
    public class EquipmentLabelAnalyzer
    {
        private IEquipmentLabel _container;

        public EquipmentLabelAnalyzer(IEquipmentLabel container)
        {
            _container = container;
            AnalyzeLabsls();
        }

        private void AnalyzeLabsls()
        {
            var labels = _container.IncludeLabels;
            IncludeEquipment = _container.IncludeLabels.Contains(Enum.GetName(EquipmentLabel.Equipment));
            IncludeComponents = _container.IncludeLabels.Contains(Enum.GetName(EquipmentLabel.Component));
            IncludeParts = _container.IncludeLabels.Contains(Enum.GetName(EquipmentLabel.Part));
        }

        public bool IncludeEquipment { get; set; }
        public bool IncludeComponents { get; set; }
        public bool IncludeParts { get; set; }
    }
}
