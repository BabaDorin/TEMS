using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.EquipmentEntities;

namespace temsAPI.Contracts
{
    interface IUnitOfWork: IDisposable
    {
        IGenericRepository<Equipment> Equipment { get; }
        IGenericRepository<EquipmentDefinition> EquipmentDefinitions { get; }
        IGenericRepository<EquipmentSpecifications> EquipmentSpecifications { get; }
        IGenericRepository<EquipmentType> EquipmentTypes { get; }
        IGenericRepository<Property> Properties { get; }
        IGenericRepository<DataType> DataType { get; }
        IGenericRepository<PropertyEquipmentTypeAssociation> PropertyEquipmentTypeAssociations { get; }
        IGenericRepository<EquipmentDefinitionKinship> EquipmentDefinitionKinships { get; }

        Task Save();
    }
}
