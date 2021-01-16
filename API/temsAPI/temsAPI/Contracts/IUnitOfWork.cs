using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Contracts
{
    interface IUnitOfWork: IDisposable
    {
        IGenericRepository<Equipment> Equipments { get; }
        IGenericRepository<EquipmentDefinition> EquipmentDefinitions { get; }
        IGenericRepository<EquipmentSpecifications> EquipmentSpecifications { get; }
        IGenericRepository<EquipmentType> EquipmentTypes { get; }
        IGenericRepository<Property> Properties { get; }
        IGenericRepository<DataType> DataTypes { get; }
        IGenericRepository<PropertyEquipmentTypeAssociation> PropertyEquipmentTypeAssociations { get; }
        IGenericRepository<EquipmentDefinitionKinship> EquipmentDefinitionKinships { get; }

        IGenericRepository<Privilege> Privileges { get; }
        IGenericRepository<RolePrivileges> RolePrivileges { get; }


        Task Save();
    }
}
