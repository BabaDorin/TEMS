using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Helpers
{
    public class ArchieveHelper
    {
        private UserManager<TEMSUser> _userManager;
        private IUnitOfWork _unitOfWork;

        public ArchieveHelper(UserManager<TEMSUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> ArchieveEquipment(string equipmentId)
        {
            try
            {
                var model = (await _unitOfWork.Equipments
                    .Find<Equipment>(
                        where: q => q.Id == equipmentId,
                        include: q => q
                        .Include(q => q.Children)
                        .Include(q => q.EquipmentAllocations)
                        .Include(q => q.Logs)))
                    .FirstOrDefault();

                if (model == null)
                    return "Invalid id provided";

                model.IsArchieved = true;

                foreach (var child in model.Children)
                {
                    var result = await ArchieveEquipment(child.Id);
                }

                foreach (var allocation in model.EquipmentAllocations)
                {
                    allocation.IsArchieved = true;
                }

                foreach (var log in model.Logs)
                {
                    log.IsArchieved = true;
                }

                await _unitOfWork.Save();
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return "An error occured while archieving equipment's related data";
            }
        }

        public async Task<string> ArchieveDefinition(string definitionId)
        {
            try
            {
                var definition = (await _unitOfWork.EquipmentDefinitions
                    .Find<EquipmentDefinition>(
                        where: q => q.Id == definitionId,
                        include: q => q
                        .Include(q => q.Children)
                        .Include(q => q.Equipment)))
                    .FirstOrDefault();

                if (definition == null)
                    return "Invalid definition id provided";

                definition.IsArchieved = true;

                foreach(var child in definition.Children)
                {
                    var result = await ArchieveDefinition(child.Id);
                }

                foreach (var eq in definition.Equipment)
                {
                    var result = await ArchieveEquipment(eq.Id);
                }

                await _unitOfWork.Save();
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return "An error occured while archieving definition's related data";
            }
        }

        public async Task<string> ArchieveProperty(string propertyId)
        {
            try
            {
                var property = (await _unitOfWork.Properties
                   .Find<Property>(
                        where: q => q.Id == propertyId,
                        include: q => q
                        .Include(q => q.EquipmentSpecifications)))
                   .FirstOrDefault();

                if (property == null)
                    return "Invalid id provided";

                property.IsArchieved = true;

                foreach(var spec in property.EquipmentSpecifications)
                {
                    spec.IsArchieved = true;
                }

                await _unitOfWork.Save();
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return "An error occured while archieving property's related data";
            }
        }
        
        public async Task<string> ArchieveType(string typeId)
        {
            try
            {
                // check if type exists
                var type = (await _unitOfWork.EquipmentTypes
                    .Find<EquipmentType>
                    (
                        where: q => q.Id == typeId,
                        include: q => q
                        .Include(q => q.Children)
                        .Include(q => q.EquipmentDefinitions)
                    )).FirstOrDefault();

                if (type == null)
                    return "The specified type does not exist";

                type.IsArchieved = true;

                foreach(var child in type.Children)
                {
                    var result = await ArchieveType(child.Id);
                }

                foreach (var def in type.EquipmentDefinitions)
                {
                    var result = await ArchieveDefinition(def.Id);
                }

                await _unitOfWork.Save();
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return "An error occured while archieving type's related data";
            }
            
        }
    }
}
