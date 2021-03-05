using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.EquipmentEntities;
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
        }
    }
}
