﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.Report;
using temsAPI.ViewModels.EquipmentDefinition;

namespace temsAPI.Data.Entities.EquipmentEntities
{
    [Index(nameof(Identifier))]
    public class EquipmentDefinition: IArchiveableItem
    {
        [Key]
        public string Id { get; set; }

        public string Identifier { get; set; }
        public double Price { get; set; }
        public string Currency { get; set; } = "lei";


#nullable enable
        [ForeignKey("EquipmentTypeID")]
        public EquipmentType? EquipmentType { get; set; }
        public string? EquipmentTypeID { get; set; }

        [ForeignKey("ParentID")]
        public EquipmentDefinition? Parent { get; set; }
        public string? ParentID { get; set; }
        public string? Description { get; set; }

#nullable disable

        public DateTime? DateArchieved { get; set; }
        private bool isArchieved;
        public bool IsArchieved
        {
            get
            {
                return isArchieved;
            }
            set
            {
                isArchieved = value;
                DateArchieved = (value)
                    ? DateTime.Now
                    : null;
            }
        }

        public virtual ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
        public virtual ICollection<EquipmentSpecifications> EquipmentSpecifications { get; set; } = new List<EquipmentSpecifications>();
        public ICollection<EquipmentDefinition> Children { get; set; } = new List<EquipmentDefinition>();
        public virtual ICollection<ReportTemplate> ReportTemplatesMemberOf { get; set; } = new List<ReportTemplate>();
    
        public static async Task<EquipmentDefinition> FromViewModel(IUnitOfWork unitOfWork, AddEquipmentDefinitionViewModel viewModel)
        {
            EquipmentDefinition equipmentDefinition = new EquipmentDefinition
            {
                Id = Guid.NewGuid().ToString(),
                Identifier = viewModel.Identifier,
                EquipmentTypeID = viewModel.TypeId,
                Price = viewModel.Price,
                Currency = viewModel.Currency,
                Description = viewModel.Description,
            };

            foreach (var property in viewModel.Properties)
            {
                equipmentDefinition.EquipmentSpecifications.Add(new EquipmentSpecifications
                {
                    Id = Guid.NewGuid().ToString(),
                    EquipmentDefinitionID = equipmentDefinition.Id,
                    PropertyID = (await unitOfWork.Properties.Find<Property>(q => q.Name == property.Value))
                        .FirstOrDefault().Id,
                    Value = property.Label,
                });
            }

            foreach(var child in viewModel.Children)
            {
                // Case 1: Child definition already existed
                if(child.Id != null)
                {
                    var childDefinition = (await unitOfWork.EquipmentDefinitions
                        .Find<EquipmentDefinition>(q => q.Id == child.Id))
                        .FirstOrDefault();

                    if (childDefinition == null)
                        throw new Exception("Invalid child definition ID provided");

                    equipmentDefinition.Children.Add(childDefinition);
                    continue;
                }

                // Case 2: Child definition has been defined now, it's new for the system
                equipmentDefinition.Children.Add(await FromViewModel(unitOfWork, child));
            }

            return equipmentDefinition;
        }
    }
}
