using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.KeyEntities;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Helpers;
using temsAPI.Services;
using temsAPI.ViewModels.Archieve;

namespace temsAPI.Data.Managers
{
    public class ArchieveManager : EntityManager
    {
        ArchieveHelper _archieveHelper;
        LogManager _logManager;
        EquipmentManager _equipmentManager;
        EquipmentDefinitionManager _equipmentDefinitionManager;
        EquipmentTypeManager _equipmentTypeManager;
        EquipmentPropertyManager _equipmentPropertyManager;
        KeyManager _keyManager;
        PersonnelManager _personnelManager;
        RoomManager _roomManager;
        TEMSUserManager _temsUserManager;
        TicketManager _ticketManager;
        SystemConfigurationService _systemConfigurationService;

        public ArchieveManager(
            IUnitOfWork unitOfWork, 
            ClaimsPrincipal user,
            LogManager logManager,
            SystemConfigurationService systemConfigurationService,
            EquipmentManager equipmentManager,
            EquipmentDefinitionManager equipmentDefinitionManager,
            EquipmentTypeManager equipmentTypeManager,
            EquipmentPropertyManager equipmentPropertyManager,
            KeyManager keyManager,
            PersonnelManager personnelManager,
            RoomManager roomManager,
            TEMSUserManager temsUserManager,
            TicketManager ticketManager
            ) : base(unitOfWork, user)
        {
            _logManager = logManager;
            _archieveHelper = new ArchieveHelper(unitOfWork, user);
            _systemConfigurationService = systemConfigurationService;
            _equipmentManager = equipmentManager;
            _equipmentDefinitionManager = equipmentDefinitionManager;
            _equipmentTypeManager = equipmentTypeManager;
            _equipmentPropertyManager = equipmentPropertyManager;
            _keyManager = keyManager;
            _personnelManager = personnelManager;
            _roomManager = roomManager;
            _temsUserManager = temsUserManager;
            _ticketManager = ticketManager;
        }

        /// <summary>
        /// Retrieves archived items of the specified item type and maps them to ArchievedItemViewModel
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public async Task<List<ArchievedItemViewModel>> GetArchievedItems(string itemType)
        {

            List<ArchievedItemViewModel> items = new();
            switch (itemType)
            {
                case "equipment":
                    items = await _archieveHelper.GetArchievedItemsFromRepo(_unitOfWork.Equipments); break;
                case "issues":
                    items = await _archieveHelper.GetArchievedItemsFromRepo(_unitOfWork.Tickets); break;
                case "rooms":
                    items = await _archieveHelper.GetArchievedItemsFromRepo(_unitOfWork.Rooms); break;
                case "personnel":
                    items = await _archieveHelper.GetArchievedItemsFromRepo(_unitOfWork.Personnel); break;
                case "keys":
                    items = await _archieveHelper.GetArchievedItemsFromRepo(_unitOfWork.Keys); break;
                case "reportTemplates":
                    items = await _archieveHelper.GetArchievedItemsFromRepo(_unitOfWork.ReportTemplates); break;
                case "equipmentAllocations":
                    items = await _archieveHelper.GetArchievedItemsFromRepo(_unitOfWork.EquipmentAllocations); break;
                case "logs":
                    items = await _archieveHelper.GetArchievedItemsFromRepo(_unitOfWork.Logs); break;
                case "keyAllocations":
                    items = await _archieveHelper.GetArchievedItemsFromRepo(_unitOfWork.KeyAllocations); break;
                case "properties":
                    items = await _archieveHelper.GetArchievedItemsFromRepo(_unitOfWork.Properties); break;
                case "equipmentTypes":
                    items = await _archieveHelper.GetArchievedItemsFromRepo(_unitOfWork.EquipmentTypes); break;
                case "equipmentDefinitions":
                    items = await _archieveHelper.GetArchievedItemsFromRepo(_unitOfWork.EquipmentDefinitions); break;
                default:
                    throw new Exception("Invalid item type");
            }

            return items;
        }

        public async Task<string> SetArchivationStatus(string itemType, string itemId, bool status)
        {
            // BEFREE: Add Archieve and Dearchive as parts of IArchievable interface and receive the item as 
            // an IArchievablem (Call Archieve or Dearchive depdending on status).

            ArchieveHelper archieveHelper = new ArchieveHelper(_unitOfWork, _user, _logManager);
            switch (itemType)
            {
                case "equipment":
                    return await archieveHelper.SetEquipmentArchivationStatus(itemId, status);
                case "issues":
                    return await archieveHelper.SetTicketArchivationStatus(itemId, status);
                case "rooms":
                    return await archieveHelper.SetRoomArchivationStatus(itemId, status);
                case "personnel":
                    return await archieveHelper.SetPersonnelArchivationStatus(itemId, status);
                case "keys":
                    return await archieveHelper.SetKeyArchivationStatus(itemId, status);
                case "reportTemplates":
                    return await archieveHelper.SetReportTemplateArchivationStatus(itemId, status);
                case "equipmentAllocations":
                    return await archieveHelper.SetEquipmenAllocationtArchivationStatus(itemId, status);
                case "logs":
                    return await archieveHelper.SetLogArchivationStatus(itemId, status);
                case "keyAllocations":
                    return await archieveHelper.SetKeyAllocationArchivationStatus(itemId, status);
                case "properties":
                    return await archieveHelper.SetPropertyArchivationStatus(itemId, status);
                case "equipmentTypes":
                    return await archieveHelper.SetTypeArchivationStatus(itemId, status);
                case "equipmentDefinitions":
                    return await archieveHelper.SetDefinitionArchivationStatus(itemId, status);
                case "user":
                    return await archieveHelper.SetUserArchivationStatus(itemId, status);
                default:
                    throw new Exception("Unknown item type");
            }
        }

        /// <summary>
        /// Finds and removed archived items that excedeed the archivation period and are eligible for
        /// deletion
        /// </summary>
        /// <returns></returns>
        public async Task RemoveOverArchivedItems()
        {
            int archivePeriodHours = _systemConfigurationService.AppSettings.ArchiveIntervalHr;

            // Remove equipment
            var equipment = (await _unitOfWork.Equipments
                .FindAll<Equipment>(
                    where: q => IsEntityEligibleForDeletion(q),
                    include: q => q.Include(q => q.Children)))
                .ToList();

            foreach (var q in equipment)
                await _equipmentManager.Remove(q);

            // Remove equipment allocations
            var allocations = (await _unitOfWork.EquipmentAllocations
                .FindAll<EquipmentAllocation>(q => IsEntityEligibleForDeletion(q)))
                .ToList();

            foreach (var q in allocations)
                await _equipmentManager.RemoveAllocation(q);

            // Remove definitions
            var definitions = (await _unitOfWork.EquipmentDefinitions
                .FindAll<EquipmentDefinition>(
                    where: q => IsEntityEligibleForDeletion(q),
                    include: q => q.Include(q => q.Children)))
                .ToList();

            foreach (var q in definitions)
                await _equipmentDefinitionManager.Remove(q);

            // Remove types
            var types = (await _unitOfWork.EquipmentTypes
                .FindAll<EquipmentType>(
                    where: q => IsEntityEligibleForDeletion(q),
                    include: q => q.Include(q => q.Children)))
                .ToList();

            foreach (var q in types)
                await _equipmentTypeManager.Remove(q);

            // Remove properties
            var properties = (await _unitOfWork.Properties
                .FindAll<Property>(q => IsEntityEligibleForDeletion(q)))
                .ToList();

            foreach (var q in properties)
                await _equipmentPropertyManager.Remove(q);

            // Remove keys
            var keys = (await _unitOfWork.Keys
                .FindAll<Key>(q => IsEntityEligibleForDeletion(q)))
                .ToList();

            foreach (var q in keys)
                await _keyManager.Remove(q);

            // Remove key allocations
            var keyAllocations = (await _unitOfWork.KeyAllocations
                .FindAll<KeyAllocation>(q => IsEntityEligibleForDeletion(q)))
                .ToList();

            foreach (var q in keyAllocations)
                await _keyManager.RemoveAllocation(q);

            // Remove personnel
            var personnel = (await _unitOfWork.Personnel
                .FindAll<Personnel>(q => IsEntityEligibleForDeletion(q)))
                .ToList();

            foreach (var q in personnel)
                await _personnelManager.Remove(q);

            // Remove rooms
            var rooms = (await _unitOfWork.Rooms
                .FindAll<Room>(q => IsEntityEligibleForDeletion(q)))
                .ToList();

            foreach (var q in rooms)
                await _roomManager.Remove(q);

            // Remove users
            var users = (await _unitOfWork.TEMSUsers
                .FindAll<TEMSUser>(q => IsEntityEligibleForDeletion(q)))
                .ToList();

            foreach (var q in users)
                await _temsUserManager.RemoveUser(q.Id);

            // Remove tickets
            var tickets = (await _unitOfWork.Tickets
                .FindAll<Ticket>(q => IsEntityEligibleForDeletion(q)))
                .ToList();

            foreach (var q in tickets)
                await _ticketManager.Remove(q);
        }

        /// <summary>
        /// Checks an entity for deletion eligibility
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>true if the item has exceeded the archivation period</returns>
        private bool IsEntityEligibleForDeletion(IArchiveable entity)
        {
            return 
                entity.IsArchieved
                && (DateTime.UtcNow - entity.DateArchieved)
                .Value.Hours >= _systemConfigurationService.AppSettings.ArchiveIntervalHr; 
        }
    }
}
