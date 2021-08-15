﻿using Microsoft.EntityFrameworkCore;
using SIC_Parser.Models;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;

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

            string validationResult = sicComputer.Validate();
            if (validationResult != null)
                return validationResult;

            // TEMSID or SerialNumber of this computer already exists
            if (await _unitOfWork.Equipments
                .isExists(q => q.TEMSID == sicComputer.TEMSID && !q.IsArchieved))
                return $"An equipment with the [{sicComputer.TEMSID}] TEMSID already exists.";

            if (await _unitOfWork.Equipments
                .isExists(q => q.SerialNumber == sicComputer.Motherboards[0].SerialNumber && !q.IsArchieved))
                return $"An equipment with the [{sicComputer.Motherboards[0].SerialNumber}] SerialNumber already exists.";
                
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
                IsUsed = sicComputer.IsUsed,
                IsDefect = sicComputer.IsDefect
            };

            if(!String.IsNullOrEmpty(sicComputer.TeamViewerID) && !String.IsNullOrEmpty(sicComputer.TeamViewerPassword))
            {
                computer.Description = String.Format(
                    "Team Viewer ID: [ {0} ], {1}Team Viewer Password: [ {2} ], {3} {4}", 
                    sicComputer.TeamViewerID, 
                    Environment.NewLine,
                    sicComputer.TeamViewerPassword,
                    Environment.NewLine,
                    sicComputer.Description);
            }

            if (computerDefinition != null)
                computer.EquipmentDefinition = computerDefinition;
            else
                computer.EquipmentDefinition = await RegisterComputerDefinition(sicComputer);

            await _unitOfWork.Save();
                
            string assignationResult = await AssignData(computer, sicComputer);
            if(assignationResult != null)
                return assignationResult;

            await _unitOfWork.Equipments.Create(computer);
            await _unitOfWork.Save();

            // BEFREE: TEST & ADD MORE VALIDATION
            return null;
        }

        /// <summary>
        /// Registers a new computer definition, based on info provided by sicCoputer model.
        /// </summary>
        /// <param name="sicComputer"></param>
        /// <returns>The equipment definition that has been created</returns>
        private async Task<EquipmentDefinition> RegisterComputerDefinition(Computer sicComputer)
        {
            var type = (await _unitOfWork.EquipmentTypes
                .Find<EquipmentType>(q => q.Name == "Computer"))
                .FirstOrDefault();

            EquipmentDefinition computerDefinition = new EquipmentDefinition
            {
                Id = Guid.NewGuid().ToString(),
                Identifier = sicComputer.Identifier,
                EquipmentType = type
            };

            await _unitOfWork.EquipmentDefinitions.Create(computerDefinition);
            await _unitOfWork.Save();

            // CPU Definitions
            type = (await _unitOfWork.EquipmentTypes
                .Find<EquipmentType>(q => q.Name == "CPU"))
                .FirstOrDefault();
            foreach (var cpu in sicComputer.CPUs)
                await AddChildDefinition(cpu, "Name", computerDefinition, type);

            // GPU Definitions
            type = (await _unitOfWork.EquipmentTypes
                .Find<EquipmentType>(q => q.Name == "GPU"))
                .FirstOrDefault();
            foreach (var gpu in sicComputer.GPUs)
                await AddChildDefinition(gpu, "Name", computerDefinition, type);

            // PSUs
            type = (await _unitOfWork.EquipmentTypes
                .Find<EquipmentType>(q => q.Name == "PSU"))
                .FirstOrDefault();
            foreach (var psu in sicComputer.PSUs)
            {
                if (String.IsNullOrEmpty(psu.SerialNumber))
                    continue;
                
                await AddChildDefinition(psu, "Model", computerDefinition, type);
            }

            // Motherboards
            type = (await _unitOfWork.EquipmentTypes
                .Find<EquipmentType>(q => q.Name == "Motherboard"))
                .FirstOrDefault();
            foreach (var motherboard in sicComputer.Motherboards)
                await AddChildDefinition(motherboard, "Product", computerDefinition, type);

            // NetworkInterfaces
            type = (await _unitOfWork.EquipmentTypes
                .Find<EquipmentType>(q => q.Name == "Network Interface"))
                .FirstOrDefault();
            foreach (var netIntf in sicComputer.NetworkInterfaces)
                await AddChildDefinition(netIntf, "Description", computerDefinition, type);

            // Monitors
            type = (await _unitOfWork.EquipmentTypes
                .Find<EquipmentType>(q => q.Name == "Monitor"))
                .FirstOrDefault();
            foreach (var monitor in sicComputer.Monitors)
            {
                if (String.IsNullOrEmpty(monitor.SerialNumber) && String.IsNullOrEmpty(monitor.TEMSID))
                    continue;

                await AddChildDefinition(monitor, "Name", computerDefinition, type);
            }

            // RAMS
            type = (await _unitOfWork.EquipmentTypes
                .Find<EquipmentType>(q => q.Name == "RAM"))
                .FirstOrDefault();
            foreach (var ram in sicComputer.RAMs)
                await AddChildDefinition(ram, "PartNumber", computerDefinition, type);

            // Storages
            type = (await _unitOfWork.EquipmentTypes
                .Find<EquipmentType>(q => q.Name == "Storage"))
                .FirstOrDefault();
            foreach (var storage in sicComputer.Storages)
                await AddChildDefinition(storage, "Caption", computerDefinition, type);

            await _unitOfWork.Save();
            return computerDefinition;
        }

        /// <summary>
        /// Given an instance of Equipment (computer) and the sicComputer model, it assigns data from
        /// sicComputer to Equipment instance.
        /// </summary>
        /// <param name="computer"></param>
        /// <param name="sicComputer"></param>
        /// <returns></returns>
        private async Task<string> AssignData(Equipment computer, Computer sicComputer)
        {
            StringBuilder stringBuilder = new StringBuilder("");
            string serialNumber;
            string temsId;

            // Motherboards
            for (int i = 0; i < sicComputer.Motherboards.Count; i++)
            {
                serialNumber = "Motherboard" + i + " " + sicComputer.Motherboards[i].SerialNumber;
                if (await _unitOfWork.Equipments
                    .isExists(q => q.SerialNumber == serialNumber && !q.IsArchieved))
                    stringBuilder.Append($"A motherboard with the [{serialNumber}] serial number already exists.\n");
                else
                    await AssignChildData(sicComputer.Motherboards[i], "Product", computer, serialNumber);
            }

            computer.SerialNumber = sicComputer.Motherboards[0].SerialNumber;

            //CPUs
            for (int i = 0; i < sicComputer.CPUs.Count; i++)
            {
                serialNumber = "CPU" + i + " " + computer.SerialNumber;
                if (await _unitOfWork.Equipments
                    .isExists(q => q.SerialNumber == serialNumber && !q.IsArchieved))
                    stringBuilder.Append($"A CPU with the [{serialNumber}] serial number already exists.\n");
                else
                    await AssignChildData(sicComputer.CPUs[i], "Name", computer, serialNumber);
            }

            // GPUs
            for (int i = 0; i < sicComputer.GPUs.Count; i++)
            {
                serialNumber = "GPU" + i + " " + computer.SerialNumber;
                if (await _unitOfWork.Equipments
                    .isExists(q => q.SerialNumber == serialNumber && !q.IsArchieved))
                    stringBuilder.Append($"A GPU with the [{serialNumber}] serial number already exists.\n");
                else
                    await AssignChildData(sicComputer.GPUs[i], "Name", computer, "GPU" + i + " " + computer.SerialNumber);
            }

            // PSUs
            for (int i = 0; i < sicComputer.PSUs.Count; i++)
            {
                var psu = sicComputer.PSUs[i];
                if (string.IsNullOrEmpty(psu.SerialNumber))
                    continue;

                serialNumber = psu.SerialNumber;
                if (await _unitOfWork.Equipments
                    .isExists(q => q.SerialNumber == serialNumber && !q.IsArchieved))
                    stringBuilder.Append($"A PSU with the [{serialNumber}] serial number already exists.\n");
                else
                    await AssignChildData(psu, "Model", computer, serialNumber);
            }

            // Network Interfaces
            for (int i = 0; i < sicComputer.NetworkInterfaces.Count; i++)
            {
                serialNumber = $"NetIntf{i}_{computer.SerialNumber}_{sicComputer.NetworkInterfaces[i].PhysicalAddress}";
                if (await _unitOfWork.Equipments
                    .isExists(q => q.SerialNumber == serialNumber && !q.IsArchieved))
                    stringBuilder.Append($"A Network Interface with the [{serialNumber}] serial number already exists.\n");
                else
                    await AssignChildData(sicComputer.NetworkInterfaces[i], "Description", computer, serialNumber);
            }

            // Monitors
            for (int i = 0; i < sicComputer.Monitors.Count; i++)
            {
                serialNumber = "Mon" + i + " " + sicComputer.Monitors[i].SerialNumber;
                temsId = sicComputer.Monitors[i].TEMSID;
                bool ok = true;

                if (await _unitOfWork.Equipments
                    .isExists(q => q.SerialNumber == serialNumber && !q.IsArchieved))
                {
                    ok = false;
                    stringBuilder.Append($"Monitor with the [{serialNumber}] serial number already exists.\n");
                }

                if(await _unitOfWork.Equipments
                    .isExists(q => q.TEMSID == temsId && !q.IsArchieved))
                {
                    ok = false;
                    stringBuilder.Append($"An equipment with the [{temsId}] TEMSID already exists.\n");
                }
                
                if(ok)
                    await AssignChildData(sicComputer.Monitors[i], "Name", computer, serialNumber);
            }

            // RAMs
            for (int i = 0; i < sicComputer.RAMs.Count; i++)
            {
                serialNumber = "RAM" + i + " " + computer.SerialNumber;
                if (await _unitOfWork.Equipments
                    .isExists(q => q.SerialNumber == serialNumber && !q.IsArchieved))
                    stringBuilder.Append($"A RAM chip with the [{serialNumber}] serial number already exists.\n");
                else
                    await AssignChildData(sicComputer.RAMs[i], "PartNumber", computer, serialNumber);
            }

            // Storages
            for (int i = 0; i < sicComputer.Storages.Count; i++)
            {
                serialNumber = $"Storage{i}_{computer.SerialNumber}_{sicComputer.Storages[i].SerialNumber}";
                if (await _unitOfWork.Equipments
                    .isExists(q => q.SerialNumber == serialNumber && !q.IsArchieved))
                    stringBuilder.Append($"A storage device with the [{serialNumber}] serial number already exists.\n");
                else
                    await AssignChildData(sicComputer.Storages[i], "Caption", computer, serialNumber);
            }

            return (stringBuilder.ToString() == "") ? null : stringBuilder.ToString();
        }

        /// <summary>
        /// Assigns data from sicEntity (child equipment, like GPU, CPU etc.) to the parent, which is the 
        /// Equipment instance of computer (parent).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">SIC child model</param>
        /// <param name="identifierPropName">Name of the property which identifies child model's definition</param>
        /// <param name="parent">Equipment instance, representing the computer</param>
        /// <param name="serialNumber">The serial number that will be assigned to child</param>
        /// <param name="TEMSID">(Optional) - child temsId (if it exists)</param>
        /// <returns></returns>
        private async Task AssignChildData<T>(T entity, string identifierPropName, Equipment parent, string serialNumber, string TEMSID = null)
        {
            string identifierValue = entity.GetType().GetProperty(identifierPropName).GetValue(entity).ToString();

            var definition = (await _unitOfWork.EquipmentDefinitions
                    .Find<EquipmentDefinition>(q => q.Identifier == identifierValue))
                    .FirstOrDefault();

            parent.Children.Add(new Equipment
            {
                Id = Guid.NewGuid().ToString(),
                SerialNumber = serialNumber,
                EquipmentDefinition = definition,
            });
        }

        /// <summary>
        /// Adds from existing or creates a new definition for child equipment.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">SIC child model</param>
        /// <param name="identifierPropName">Name of the property which identifies child model's definition</param>
        /// <param name="parentDefinition">computer's definition</param>
        /// <returns></returns>
        private async Task AddChildDefinition<T>(T entity, string identifierPropName, EquipmentDefinition parentDefinition, EquipmentType type)
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
                Parent = parentDefinition,
                EquipmentType = type
            };

            await AddDefinitionSpecifications(def, entity);
            await _unitOfWork.EquipmentDefinitions.Create(def);
            await _unitOfWork.Save();
        }

        /// <summary>
        /// Adds definition specifications (property-value associations) based on the provided
        /// entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="definition">Definition to be modified</param>
        /// <param name="entity">SIC Child model</param>
        /// <returns></returns>
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
