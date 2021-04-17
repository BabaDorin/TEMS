﻿using Microsoft.EntityFrameworkCore;
using SIC_Parser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Validation;

namespace temsAPI.Services.SICServices
{
    internal class SIC_Register
    {
        IUnitOfWork _unitOfWork;
        public SIC_Register(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Registers a computer based on data being provided by the model (sicComputer)
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="sicComputer">Computer model generated by SIC Parser</param>
        /// <returns>Returns null if everything is ok, otherwise - error message.</returns>
        public async Task<string> RegisterComputer(Computer sicComputer)
        {
            // Step 1: Register the computer itself.
            // Validation:
            sicComputer.TEMSID = sicComputer.TEMSID.Trim();
            if (String.IsNullOrEmpty(sicComputer.TEMSID))
                return "Computer TEMSID is Mandatory!";

            sicComputer.Identifier = sicComputer.Identifier.Trim();
            if (String.IsNullOrEmpty(sicComputer.Identifier))
                return "Computer identifier is Mandatory!";

            var computerType = (await _unitOfWork.EquipmentTypes
                .Find<EquipmentType>(
                    where: q => q.Name == "Computer",
                    include: q => q.Include(q => q.EquipmentDefinitions)
                 )).FirstOrDefault();

            var computerDefinition = computerType.EquipmentDefinitions
                .FirstOrDefault(q => q.Identifier == sicComputer.Identifier);

            var computer = new Equipment
            {
                Id = Guid.NewGuid().ToString(),
                TEMSID = sicComputer.TEMSID,
            };

            // Process of registering a computer:
            // 1) Create an equipment with name Computer. Set ID.
            // 2) Find out if computer definition exists. If not - create it.
            // 2.1) How to create a computer definition?
            //      Create a definition with the specified identifier.
            //      Foreach sicComputer child, if not null, check if child definition exists, if so => set it.
            //      Otherwise - create the definition and set it.
            //      Add the definition as child for computer definition.
            //      Save definition.
            // 3) Register the equipment itsels by providing temsid and serial numbers.
            //      Computer serial number is motherboard serial number.
            //      then, foreach children, do the following:
            //      a. check if there is a serialNumber property. If so,
            //      check if there isn't any equipment with the same id already registered.
            //      if there is >>
            //         Add [Type]_[index]_[serialNumber] as serial number
            //      if there isn't >> set the serial number.
            //   Add description for types that have this property
            //   Description for computer will be TV data.

            if (computerDefinition != null)
                computer.EquipmentDefinition = computerDefinition;
            else
                computer.EquipmentDefinition = await RegisterComputerDefinition(sicComputer);

            return null;
        }

        private async Task<EquipmentDefinition> RegisterComputerDefinition(Computer sicComputer)
        {
            var computerType = (await _unitOfWork.EquipmentTypes
                .Find<EquipmentType>(q => q.Name == "Computer"))
                .FirstOrDefault();

            EquipmentDefinition computerDefinition = new EquipmentDefinition
            {
                Id = Guid.NewGuid().ToString(),
                Identifier = sicComputer.Identifier,
                EquipmentType = computerType
            };

            // CPU Definitions
            foreach(var cpu in sicComputer.CPUs)
                await AddChildDefinition(cpu, "Name", computerDefinition);

            // GPU Definitions
            foreach (var gpu in sicComputer.GPUs)
                await AddChildDefinition(gpu, "Name", computerDefinition);

            // PSUs
            foreach (var psu in sicComputer.PSUs)
            {
                if (String.IsNullOrEmpty(psu.SerialNumber))
                    continue;
                
                await AddChildDefinition(psu, "Model", computerDefinition);
            }

            // Motherboards
            foreach(var motherBoard in sicComputer.Motherboards)
                await AddChildDefinition(motherBoard, "Product", computerDefinition);

            // NetworkInterfaces
            foreach (var netIntf in sicComputer.NetworkInterfaces)
                await AddChildDefinition(netIntf, "Description", computerDefinition);

            // Monitors
            foreach (var monitor in sicComputer.Monitors)
            {
                if (String.IsNullOrEmpty(monitor.SerialNumber) && String.IsNullOrEmpty(monitor.TEMSID))
                    continue;

                await AddChildDefinition(monitor, "Name", computerDefinition);
            }

            // RAMS
            foreach (var ram in sicComputer.RAMs)
                await AddChildDefinition(ram, "PartNumber", computerDefinition);

            // Storages
            foreach (var storage in sicComputer.Storages)
                await AddChildDefinition(storage, "Caption", computerDefinition);

            await _unitOfWork.Save();
            return computerDefinition;
        }

        private async Task AddChildDefinition<T>(T entity, string identifierPropName, EquipmentDefinition parentDefinition)
        {
            string identifierValue = entity.GetType().GetProperty(identifierPropName).GetValue(entity).ToString();
            EquipmentDefinition def = (await _unitOfWork.EquipmentDefinitions
                    .Find<EquipmentDefinition>(q => q.Identifier == identifierValue))
                    .FirstOrDefault();

            if (def != null)
            {
                parentDefinition.Children.Add(def);
                return;
            }

            def = new EquipmentDefinition
            {
                Id = Guid.NewGuid().ToString(),
                Identifier = identifierValue,
            };

            await AddDefinitionSpecifications(def, entity);
        }

        private async Task AddDefinitionSpecifications<T>(EquipmentDefinition definition, T entity)
        {
            foreach (var prop in entity.GetType().GetProperties())
            {
                var property = (await _unitOfWork.Properties
                    .Find<Property>(
                        where: q => q.Name == prop.Name,
                        include: q => q.Include(q => q.DataType)
                    )).FirstOrDefault();

                if (property == null)
                    continue;

                
                if(property.DataType.Name.ToLower() == "number")
                {
                    double testVal;
                    if(!double.TryParse(prop.GetValue(entity).ToString(), out testVal))
                    {
                        throw new Exception($"Property: {prop.Name} is of type number, but it's value: " +
                            $"{prop.GetValue(entity).ToString()} is of another type.");
                    }
                }

                definition.EquipmentSpecifications.Add(new EquipmentSpecifications
                {
                    Id = Guid.NewGuid().ToString(),
                    EquipmentDefinition = definition,
                    Property = property,
                    Value = prop.GetValue(entity).ToString(),
                });
            }
        }
    }
}
