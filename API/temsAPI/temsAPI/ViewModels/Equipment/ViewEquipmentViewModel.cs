using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using temsAPI.ViewModels.EquipmentDefinition;
using temsAPI.ViewModels.EquipmentType;
using temsAPI.ViewModels.Property;

namespace temsAPI.ViewModels.Equipment
{
    public class ViewEquipmentViewModel
    {
        public string Id { get; set; }
        public EquipmentDefinitionViewModel Definition { get; set; }
        public string TemsId { get; set; }
        public string SerialNumber { get; set; }
        public IOption Room { get; set; }
        public IOption Personnnel { get; set; }
        public ViewEquipmentTypeViewModel Type { get; set; }
        public List<ViewPropertyViewModel> SpecificProperties { get; set; }
        public List<Option> Children { get; set; }
        public IOption Parent { get; set; }
        public bool IsUsed { get; set; }
        public bool IsDefect { get; set; }
        public bool IsArchieved { get; set; }
        public List<string> Photos { get; set; }

        public ViewEquipmentViewModel()
        {
            SpecificProperties = new List<ViewPropertyViewModel>();
            Children = new List<Option>();
            Photos = new List<string>();
        }

        public static ViewEquipmentViewModel ParseEquipment(IMapper  mapper, Data.Entities.EquipmentEntities.Equipment model)
        {
            var activeRoomAllocation = model.EquipmentAllocations
                    .Where(q => q.DateReturned == null && q.RoomID != null)
                    ?.FirstOrDefault();

            var activePersonnelAllocation = model.EquipmentAllocations
                .Where(q => q.DateReturned == null && q.PersonnelID != null)
                ?.FirstOrDefault();

            ViewEquipmentViewModel viewModel = new ViewEquipmentViewModel
            {
                Id = model.Id,
                Definition = EquipmentDefinitionViewModel.FromModel(model.EquipmentDefinition),
                IsDefect = model.IsDefect,
                IsUsed = model.IsUsed,
                IsArchieved = model.IsArchieved,
                SerialNumber = model.SerialNumber,
                TemsId = model.TEMSID,
                Type = ViewEquipmentTypeViewModel.FromModel(model.EquipmentDefinition.EquipmentType),
                Personnnel = (activePersonnelAllocation == null)
                        ? null
                        : new Option
                        {
                            Value = activePersonnelAllocation.PersonnelID,
                            Label = activePersonnelAllocation.Personnel.Name
                        },
                Room = (activeRoomAllocation == null)
                        ? null
                        : new Option
                        {
                            Value = activeRoomAllocation.RoomID,
                            Label = activeRoomAllocation.Room.Identifier
                        },
                Parent = (model.Parent == null)
                        ? null
                        : new Option
                        {
                            Value = model.Parent.Id,
                            Label = model.Parent.EquipmentDefinition.Identifier,
                        },
                Children = (model.Children == null)
                        ? null
                        : model.Children
                        .Select(q => new Option
                        {
                            Value = q.Id,
                            Label = q.EquipmentDefinition.Identifier,
                            Additional = q.EquipmentDefinition.EquipmentType.Name
                        }).ToList(),
            };

            return viewModel;
        }
    }
}
