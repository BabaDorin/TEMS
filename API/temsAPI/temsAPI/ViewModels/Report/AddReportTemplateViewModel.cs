using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Helpers;

namespace temsAPI.ViewModels.Report
{
    public class AddReportTemplateViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Subject { get; set; }
        public List<Option> Types { get; set; } = new List<Option>();
        public List<Option> Definitions { get; set; } = new List<Option>();
        public List<Option> Personnel { get; set; } = new List<Option>();
        public List<Option> Rooms { get; set; } = new List<Option>();
        public string SepparateBy { get; set; }
        public List<string> CommonProperties { get; set; } = new List<string>();
        public List<SpecificPropertyWrapper> SpecificProperties { get; set; } = new List<SpecificPropertyWrapper>();
        public List<string> Properties { get; set; } = new List<string>();
        public string Header { get; set; }
        public string Footer { get; set; }
        public List<Option> Signatories { get; set; } = new List<Option>();

        public async Task<string> Validate(IUnitOfWork unitOfWork)
        {
            // Invalid id provided (When it's the udpate case)
            if (Id != null && !await unitOfWork.ReportTemplates
                .isExists(q => q.Id == Id))
                return "Invalid id provided";

            // Invalid subject
            if (new List<string>() { "equipment", "rooms", "personnel", "allocations" }
                .IndexOf(Subject) == -1)
                return "Invalid subject";

            // Invalid types provided
            if (Types != null)
                foreach (var item in Types)
                    if (!await unitOfWork.EquipmentTypes.isExists(q => q.Id == item.Value))
                        return $"{item.Label} is not a valid type";

            // Invalid definitions provided
            if (Definitions != null)
                foreach (var item in Definitions)
                    if (!await unitOfWork.EquipmentDefinitions.isExists(q => q.Id == item.Value))
                        return $"{item.Label} is not a valid definition";

            // Invalid personnel provided
            if (Personnel != null)
                foreach (var item in Personnel)
                    if (!await unitOfWork.Personnel.isExists(q => q.Id == item.Value))
                        return $"{item.Label} is not a valid personnel";

            // Invalid rooms provided
            if (Rooms != null)
                foreach (var item in Rooms)
                    if (!await unitOfWork.Rooms.isExists(q => q.Id == item.Value))
                        return $"{item.Label} is not a valid room";

            // Invalid signatories provided
            if (Signatories != null)
                foreach (var item in Signatories)
                    if (!await unitOfWork.Personnel.isExists(q => q.Id == item.Value))
                        return $"{item.Label} is not a valid type";

            // Invalid SepparateBy
            if (new List<string>() { "none", "room", "personnel", "type", "definition" }
                .IndexOf(SepparateBy) == -1)
                return "Invalid SepparateBy";

            // Invalid properties
            if (CommonProperties != null)
                foreach (var item in CommonProperties)
                    if (!ReportHelper.UniversalProperties.Contains(item) && !await unitOfWork.Properties.isExists(q => q.Name == item))
                        return $"{item} is not a valid property";

            var specificProperties = SpecificProperties?.SelectMany(q => q.Properties).ToList();
            if (specificProperties != null)
                foreach (var item in specificProperties)
                    if (!await unitOfWork.Properties.isExists(q => q.Name == item))
                        return $"{item} is not a valid property";

            return null;
        }
    }

    public class SpecificPropertyWrapper
    {
        public string Type { get; set; }
        public List<string> Properties { get; set; } = new List<string>();
    }
}
