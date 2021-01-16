using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using temsAPI.Data.Entities.EquipmentEntities;

namespace temsAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //// Equipment related entities
        //public DbSet<DataType> DataTypes { get; set; }
        //public DbSet<Property> Properties { get; set; }
        //public DbSet<EquipmentType> EquipmentTypes { get; set; }
        //public DbSet<PropertyEquipmentTypeAssociation> PropertyEquipmentTypeAssociations { get; set; }
        //public DbSet<EquipmentSpecifications> EquipmentSpecifications { get; set; }
        //public DbSet<EquipmentDefinition> EquipmentDefinitions { get; set; }
        //public DbSet<Equipment> Equipments { get; set; }
        //public DbSet<EquipmentDefinitionKinship> EquipmentDefinitionKinships { get; set; }
    }
}
