using AutoMapper;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.ViewModels.EquipmentDefinition;
using temsAPI.ViewModels.EquipmentType;
using temsAPI.ViewModels.Property;

namespace temsAPI.Mappings
{
    public class Maps : Profile
    {
        public Maps()
        {
            CreateMap<Property, PropertyViewModel>().ReverseMap();
            CreateMap<EquipmentType, EquipmentTypeViewModel>().ReverseMap();
            CreateMap<EquipmentDefinition, EquipmentDefinitionViewModel>().ReverseMap();
        }
    }
}
