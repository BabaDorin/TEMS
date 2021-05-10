using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.Report;
using temsAPI.Data.Entities.UserEntities;
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
        public string SeparateBy { get; set; }
        public List<string> CommonProperties { get; set; } = new List<string>();
        public List<SpecificPropertyWrapper> SpecificProperties { get; set; } = new List<SpecificPropertyWrapper>();
        public List<string> Properties { get; set; } = new List<string>();
        public string Header { get; set; }
        public string Footer { get; set; }
        public List<Option> Signatories { get; set; } = new List<Option>();


        public async Task<ReportTemplate> ToModel(IUnitOfWork unitOfWork, TEMSUser author)
        {
            List<string> typeIds = Types?.Select(q => q.Value).ToList();
            List<string> definitionIds = Definitions?.Select(q => q.Value).ToList();
            List<string> roomIds = Rooms?.Select(q => q.Value).ToList();
            List<string> personnelIds = Personnel?.Select(q => q.Value).ToList();
            List<string> specificProperties = SpecificProperties?
                .Where(q => Types.Any(q1 => q1.Label == q.Type))
                .SelectMany(q => q.Properties).ToList();
            List<string> propertyIds = CommonProperties?
                .Concat(specificProperties ?? new List<string>())
                .ToList();
            List<string> commonProperties = CommonProperties?
                .Where(q => ReportHelper.CommonProperties.Contains(q.ToLower()))
                .Select(q => q.ToLower())
                .ToList();
            List<string> signatoriesIds = Signatories?.Select(q => q.Value).ToList();

            ReportTemplate model = new ReportTemplate
            {
                Id = Guid.NewGuid().ToString(),
                Name = Name,
                Description = Description,
                Subject = Subject,
                EquipmentTypes = (typeIds != null)
                    ? (await unitOfWork.EquipmentTypes
                    .FindAll<Data.Entities.EquipmentEntities.EquipmentType>(q => typeIds.Contains(q.Id)))
                    .ToList()
                    : new List<Data.Entities.EquipmentEntities.EquipmentType>(),
                EquipmentDefinitions = (definitionIds != null)
                    ? (await unitOfWork.EquipmentDefinitions
                    .FindAll<Data.Entities.EquipmentEntities.EquipmentDefinition>(q => definitionIds.Contains(q.Id)))
                    .ToList()
                    : new List<Data.Entities.EquipmentEntities.EquipmentDefinition>(),
                Rooms = (roomIds != null)
                    ? (await unitOfWork.Rooms
                    .FindAll<Data.Entities.OtherEntities.Room>(q => roomIds.Contains(q.Id)))
                    .ToList()
                    : new List<Data.Entities.OtherEntities.Room>(),
                Personnel = (personnelIds != null)
                    ? (await unitOfWork.Personnel
                    .FindAll<Data.Entities.OtherEntities.Personnel>(q => personnelIds.Contains(q.Id)))
                    .ToList()
                    : new List<Data.Entities.OtherEntities.Personnel>(),
                SeparateBy = SeparateBy,
                Properties = (propertyIds != null)
                    ? (await unitOfWork.Properties
                    .FindAll<Data.Entities.EquipmentEntities.Property>(q => propertyIds.Contains(q.Name)))
                    .ToList()
                    : new List<Data.Entities.EquipmentEntities.Property>(),
                Header = Header,
                Footer = Footer,
                Signatories = (signatoriesIds != null)
                    ? (await unitOfWork.Personnel
                    .FindAll<Data.Entities.OtherEntities.Personnel>(q => signatoriesIds.Contains(q.Id)))
                    .ToList()
                    : new List<Data.Entities.OtherEntities.Personnel>(),
                CreatedBy = (await unitOfWork.TEMSUsers
                    .Find<Data.Entities.UserEntities.TEMSUser>(
                        where: q => q.Id == author.Id
                    )).FirstOrDefault(),
                DateCreated = DateTime.Now,
                CommonProperties = (commonProperties.Count > 0)
                    ? String.Join(" ", commonProperties)
                    : null
            };

            if (author.Email == "tems@admin")
            {
                model.CreatedBy = null;
                model.CreatedById = null;
            }

            return model;
        }
        
        public static AddReportTemplateViewModel FromModel(ReportTemplate template)
        {
            var viewModel = new AddReportTemplateViewModel
            {
                Id = template.Id,
                Name = template.Name,
                Description = template.Description,
                Subject = template.Subject,
                Types = template.EquipmentTypes.Select(q => new Option
                {
                    Value = q.Id,
                    Label = q.Name
                }).ToList(),
                Definitions = template.EquipmentDefinitions.Select(q => new Option
                {
                    Value = q.Id,
                    Label = q.Identifier
                }).ToList(),
                Rooms = template.Rooms.Select(q => new Option
                {
                    Value = q.Id,
                    Label = q.Identifier
                }).ToList(),
                Personnel = template.Personnel.Select(q => new Option
                {
                    Value = q.Id,
                    Label = q.Name
                }).ToList(),
                Properties = template.Properties.Select(q => q.Name).ToList(),
                SeparateBy = template.SeparateBy,
                Header = template.Header,
                Footer = template.Footer,
                Signatories = template.Signatories.Select(q => new Option
                {
                    Value = q.Id,
                    Label = q.Name
                }).ToList(),
            };

            if (template.CommonProperties != null)
                viewModel.Properties = viewModel.Properties
                    .Concat(template.CommonProperties.Split(' '))
                    .ToList();

            return viewModel;
        }

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
                        return $"{item.Label} is not a valid personnel";

            // Invalid SepparateBy
            if (new List<string>() { "none", "room", "personnel", "type", "definition" }
                .IndexOf(SeparateBy) == -1)
                return "Invalid SepparateBy";

            // Invalid properties
            if (CommonProperties != null)
                for(int i = 0; i < CommonProperties.Count; i++)
                {
                    CommonProperties[i] = CommonProperties[i].ToLower();
                    if (!ReportHelper.CommonProperties.Contains(CommonProperties[i]) 
                        && !await unitOfWork.Properties.isExists(q => q.Name == CommonProperties[i]))
                        return $"{CommonProperties[i]} is not a valid property";
                }

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
