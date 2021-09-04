using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Helpers.ReusableSnippets;
using temsAPI.System_Files;

namespace temsAPI.Data.Managers
{
    public class EquipmentLabelManager : IEquipmentLabelManager
    {
        readonly IUnitOfWork _unitOfWork;
        readonly IEquipmentLabelHelper _labelHelper;

        public EquipmentLabelManager(
            IUnitOfWork unitOfWork,
            IEquipmentLabelHelper labelHelper)
        {
            _unitOfWork = unitOfWork;
            _labelHelper = labelHelper;
        }

        /// <summary>
        /// Sets equipment label. If equipment has children, they will get labeled as well.
        /// </summary>
        /// <param name="equipment"></param>
        /// <returns></returns>
        public async Task SetLabel(Equipment equipment)
        {
            if (equipment.EquipmentDefinition == null
                || equipment.EquipmentDefinition.EquipmentType == null
                || equipment.EquipmentDefinition.EquipmentType.Parents.IsNullOrEmpty())
            {
                // If Parents is empty (not null), we will fetch the type one more time 
                // with parents included just to be sure.
                equipment.EquipmentDefinition = await GetEquipmentDefinition(equipment);
            }

            equipment.SetLabel(_labelHelper.GetAppropriateLabel(equipment));

            if (equipment.Children.IsNullOrEmpty())
            {
                // Fetch children along with their relevant information (definiton, type, parents).
                equipment.Children = (await GetChildren(equipment)).ToList();
            }

            if (!equipment.Children.IsNullOrEmpty())
            {
                foreach(var child in equipment.Children)
                {
                    // In some cases, parentId is not set (Ex. When creating an equipment, the parentID is not set
                    // because at the moment of creation, parent does not exist (First children are inserted, then - parent)
                    child.ParentID = equipment.Id;
                    await SetLabel(child);
                }
            }
        }

        /// <summary>
        /// Set a label directly (Must be used with caution)
        /// </summary>
        /// <param name="equipment"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public void SetLabel(Equipment equipment, EquipmentLabel label)
        {
            equipment.SetLabel(label);
        }

        /// <summary>
        /// </summary>
        /// <param name="equipment"></param>
        /// <returns>Returns equipment's definition, with type information included</returns>
        private async Task<EquipmentDefinition> GetEquipmentDefinition(Equipment equipment)
        {
            return (await _unitOfWork.EquipmentDefinitions
                .Find<EquipmentDefinition>(
                    where: q => q.Id == equipment.EquipmentDefinitionID,
                    include: q => q.Include(q => q.EquipmentType)
                                   .ThenInclude(q => q.Parents)))
                .FirstOrDefault();
        }

        /// <summary>
        /// </summary>
        /// <param name="equipment"></param>
        /// <returns>Returns equipment's children along with their definitions => types => type parents list included</returns>
        private async Task<IEnumerable<Equipment>> GetChildren(Equipment equipment)
        {
            return await _unitOfWork.Equipments
                .FindAll<Equipment>(
                    where: q => q.ParentID == equipment.Id,
                    include: q => q.Include(q => q.EquipmentDefinition)
                                  .ThenInclude(q => q.EquipmentType)
                                  .ThenInclude(q => q.Parents));
        }
    }
}
