using AutoMapper;
using System.Collections.Generic;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.KeyEntities;
using temsAPI.Data.Entities.LibraryEntities;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.ViewModels.Equipment;
using temsAPI.ViewModels.EquipmentDefinition;
using temsAPI.ViewModels.EquipmentType;
using temsAPI.ViewModels.IdentityViewModels;
using temsAPI.ViewModels.Key;
using temsAPI.ViewModels.Library;
using temsAPI.ViewModels.Log;
using temsAPI.ViewModels.Property;
using temsAPI.ViewModels.Status;

namespace temsAPI.Mappings
{
    public class Maps : Profile
    {
        public Maps()
        {
            CreateMap<Property, PropertyViewModel>().ReverseMap();
            CreateMap<EquipmentType, ViewEquipmentTypeViewModel>().ReverseMap();
            CreateMap<EquipmentDefinition, EquipmentDefinitionViewModel>().ReverseMap();
            CreateMap<Equipment, AddEquipmentViewModel>().ReverseMap();
            CreateMap<List<Equipment>, List<ViewEquipmentSimplifiedViewModel>>().ReverseMap();
            CreateMap<List<Log>, List<ViewLogViewModel>>().ReverseMap();
            CreateMap<List<Status>, List<ViewStatusViewModel>>().ReverseMap();
            CreateMap<Key, AddKeyViewModel>().ReverseMap();
            CreateMap<LibraryItem, ViewLibraryItemViewModel>().ReverseMap();
            CreateMap<TEMSUser, RegisterViewModel>().ReverseMap();
        }
    }
}
