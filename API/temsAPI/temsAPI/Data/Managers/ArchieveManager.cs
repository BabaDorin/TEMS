using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Helpers;
using temsAPI.ViewModels.Archieve;

namespace temsAPI.Data.Managers
{
    public class ArchieveManager : EntityManager
    {
        private ArchieveHelper _archieveHelper;
        public ArchieveManager(IUnitOfWork unitOfWork, ClaimsPrincipal user) : base(unitOfWork, user)
        {
            _archieveHelper = new ArchieveHelper(unitOfWork, user);
        }

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

            ArchieveHelper archieveHelper = new ArchieveHelper(_unitOfWork, _user);
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
    }
}
