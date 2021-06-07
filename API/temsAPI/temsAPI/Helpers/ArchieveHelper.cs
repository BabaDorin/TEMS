using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.KeyEntities;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.Report;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.ViewModels;
using temsAPI.ViewModels.Archieve;

namespace temsAPI.Helpers
{
    public class ArchieveHelper
    {
        private IUnitOfWork _unitOfWork;
        private ClaimsPrincipal _user;

        public ArchieveHelper(IUnitOfWork unitOfWork, ClaimsPrincipal user)
        {
            _unitOfWork = unitOfWork;
            _user = user;
        }

        public async Task<List<ArchievedItemViewModel>> GetArchievedItemsFromRepo<T>(IGenericRepository<T> repo) where T : class, IArchiveableItem
        {
            return (await repo.FindAll<ArchievedItemViewModel>(
                    where: q => q.IsArchieved,
                    select: q => new ArchievedItemViewModel
                    {
                        Id = q.Id,
                        Identifier = q.Identifier,
                        DateArchieved = (DateTime)q.DateArchieved
                    }
                )).ToList();
        }

        public async Task<string> SetEquipmentArchivationStatus(string equipmentId, bool status)
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

                model.IsArchieved = status;

                foreach (var child in model.Children)
                {
                    var result = await SetEquipmentArchivationStatus(child.Id, status);
                }

                foreach (var allocation in model.EquipmentAllocations)
                {
                    allocation.IsArchieved = status;
                }

                foreach (var log in model.Logs)
                {
                    log.IsArchieved = status;
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

        public async Task<string> SetEquipmenAllocationtArchivationStatus(string allocationId, bool status)
        {
            try
            {
                var allocation = (await _unitOfWork.EquipmentAllocations
                    .Find<EquipmentAllocation>(
                        where: q => q.Id == allocationId))
                    .FirstOrDefault();

                if (allocation == null)
                    return "Invalid allocation id provided";

                allocation.IsArchieved = status;
                
                await _unitOfWork.Save();
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return "An error occured while archieving allocation's related data";
            }
        }

        public async Task<string> SetRoomArchivationStatus(string roomId, bool status)
        {
            try
            {
                var model = (await _unitOfWork.Rooms
                    .Find<Room>(
                        where: q => q.Id == roomId,
                        include: q => q
                        .Include(q => q.EquipmentAllocations)
                        .Include(q => q.Supervisories)
                        .Include(q => q.Keys)
                        .Include(q => q.Logs)))
                    .FirstOrDefault();

                if (model == null)
                    return "Invalid id provided";

                model.IsArchieved = status;

                foreach (var allocation in model.EquipmentAllocations)
                {
                    allocation.IsArchieved = status;
                }

                foreach (var key in model.Keys)
                {
                    await SetKeyArchivationStatus(key.Id, status);
                }

                foreach (var log in model.Logs)
                {
                    log.IsArchieved = status;
                }

                await _unitOfWork.Save();
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return "An error occured while archieving rooms's related data";
            }
        }

        public async Task<string> SetKeyArchivationStatus(string keyId, bool status)
        {
            try
            {
                var model = (await _unitOfWork.Keys
                    .Find<Key>(
                        where: q => q.Id == keyId,
                        include: q => q
                        .Include(q => q.KeyAllocations)))
                    .FirstOrDefault();

                if (model == null)
                    return "Invalid id provided";

                model.IsArchieved = status;

                foreach (var allocation in model.KeyAllocations)
                    model.IsArchieved = status;
                
                await _unitOfWork.Save();
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return "An error occured while archieving key's related data";
            }
        }

        public async Task<string> SetKeyAllocationArchivationStatus(string allocationId, bool status)
        {
            try
            {
                var model = (await _unitOfWork.KeyAllocations
                    .Find<KeyAllocation>(
                        where: q => q.Id == allocationId))
                    .FirstOrDefault();

                if (model == null)
                    return "Invalid id provided";

                model.IsArchieved = status;
                await _unitOfWork.Save();
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return "An error occured while archieving the allocation";
            }
        }

        public async Task<string> SetDefinitionArchivationStatus(string definitionId, bool status)
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

                definition.IsArchieved = status;

                foreach(var child in definition.Children)
                {
                    var result = await SetDefinitionArchivationStatus(child.Id, status);
                }

                foreach (var eq in definition.Equipment)
                {
                    var result = await SetEquipmentArchivationStatus(eq.Id, status);
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

        public async Task<string> SetPropertyArchivationStatus(string propertyId, bool status)
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

                property.IsArchieved = status;

                foreach(var spec in property.EquipmentSpecifications)
                {
                    spec.IsArchieved = status;
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
        
        public async Task<string> SetTypeArchivationStatus(string typeId, bool status)
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

                type.IsArchieved = status;

                foreach(var child in type.Children)
                {
                    var result = await SetTypeArchivationStatus(child.Id, status);
                }

                foreach (var def in type.EquipmentDefinitions)
                {
                    var result = await SetDefinitionArchivationStatus(def.Id, status);
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

        public async Task<string> SetLogArchivationStatus(string logId, bool status)
        {
            try
            {
                var log = (await _unitOfWork.Logs
                    .Find<Log>
                    (
                        where: q => q.Id == logId
                    )).FirstOrDefault();

                if (log == null)
                    return "The specified log does not exist";

                log.IsArchieved = status;

                await _unitOfWork.Save();
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return "An error occured while archieving log's related data";
            }
        }

        public async Task<string> SetPersonnelArchivationStatus(string personnelId, bool status)
        {
            try
            {
                var personnel = (await _unitOfWork.Personnel
                    .Find<Personnel>
                    (
                        where: q => q.Id == personnelId,
                        include: q => q
                        .Include(q => q.RoomsSupervisoried)
                    )).FirstOrDefault();

                if (personnel == null)
                    return "The specified personnel does not exist";



                personnel.IsArchieved = status;

                foreach(var item in personnel.RoomsSupervisoried)
                {
                    item.IsArchieved = status;
                }

                await _unitOfWork.Save();
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return "An error occured while archieving personnel's related data";
            }
        }

        public async Task<string> SetTicketArchivationStatus(string ticketId, bool status)
        {
            try
            {
                var ticket = (await _unitOfWork.Tickets
                    .Find<Ticket>
                    (
                        where: q => q.Id == ticketId
                    )).FirstOrDefault();

                if (ticket == null)
                    return "Invalid ticket id";

                ticket.IsArchieved = status;

                await _unitOfWork.Save();
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return "An error occured while archieving personnel's related data";
            }
        }

        public async Task<string> SetReportTemplateArchivationStatus(string templateId, bool status)
        {
            try
            {
                var template = (await _unitOfWork.ReportTemplates
                    .Find<ReportTemplate>(
                        q => q.Id == templateId
                    )).FirstOrDefault();

                if (template == null)
                    return "Invalid id provided";

                template.IsArchieved = status;
                await _unitOfWork.Save();

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return "An error occured while archieving template's related data";
            }
        }
    }
}
