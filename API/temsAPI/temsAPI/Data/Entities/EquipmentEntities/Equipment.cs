using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Helpers;
using temsAPI.Services;
using temsAPI.ViewModels.Equipment;

namespace temsAPI.Data.Entities.EquipmentEntities
{
    [Index(nameof(TEMSID))]
    [Index(nameof(SerialNumber))]
    public class Equipment : IArchiveableItem
    {
        [Key]
        public string Id { get; set; }

#nullable enable
        [ForeignKey("ParentID")]
        public Equipment? Parent { get; set; }
        public string? ParentID { get; set; }

        public string? TEMSID { get; set; }
        public string? SerialNumber { get; set; }
        public double? Price { get; set; }
        public string? Currency { get; set; }
        public string? Description { get; set; }

        [ForeignKey("EquipmentDefinitionID")]
        public EquipmentDefinition? EquipmentDefinition { get; set; }
        public string? EquipmentDefinitionID { get; set; }

        [InverseProperty("RegisteredEquipment")]
        [ForeignKey("RegisteredByID")]
        public TEMSUser? RegisteredBy { get; set; }
        public string? RegisteredByID { get; set; }

        [InverseProperty("ArchievedEquipment")]
        [ForeignKey("ArchievedById")]
        public TEMSUser? ArchievedBy { get; set; }
        public string? ArchievedById { get; set; }

        public DateTime? PurchaseDate { get; set; }
        public DateTime? RegisterDate { get; set; }
        public DateTime? DeletedDate { get; set; }
#nullable disable

        public bool IsDefect { get; set; }
        public bool IsUsed { get; set; }
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

        [NotMapped]
        public string TemsIdOrSerialNumber { get { return TEMSID ?? SerialNumber; } }
        
        [NotMapped]
        public string Identifier { get => this.TemsIdOrSerialNumber; }

        public virtual ICollection<Log> Logs { get; set; } = new List<Log>();
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public virtual ICollection<Equipment> Children { get; set; } = new List<Equipment>();
        public virtual ICollection<EquipmentAllocation> EquipmentAllocations { get; set; } = new List<EquipmentAllocation>();
    
        public static Equipment FromViewModel(ClaimsPrincipal createdBy, AddEquipmentViewModel viewModel)
        {
            Equipment model = new Equipment
            {
                Id = Guid.NewGuid().ToString(),
                TEMSID = viewModel.Temsid,
                SerialNumber = viewModel.SerialNumber,
                Currency = viewModel.Currency,
                PurchaseDate = viewModel.PurchaseDate,
                Description = viewModel.Description,
                EquipmentDefinitionID = viewModel.EquipmentDefinitionID,
                IsDefect = viewModel.IsDefect,
                IsUsed = viewModel.IsUsed,
                Price = viewModel.Price,
                RegisterDate = DateTime.Now,
                RegisteredByID = IdentityService.GetUserId(createdBy),
            };

            foreach(var child in viewModel.Children)
            {
                model.Children.Add(FromViewModel(createdBy, child));
            }

            return model;
        }
    }
}
