using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Helpers.ReusableSnippets;

namespace temsAPI.ViewModels.EquipmentType
{
    public class AddEquipmentTypeViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Option> Parents { get; set; } = new List<Option>();
        public virtual ICollection<Option> Properties{ get; set; } = new List<Option>();

        /// <summary>
        /// Validates the view model. Returns null if everything is allright, otherwise - 
        /// returns the issue.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public async Task<string> Validate(IUnitOfWork unitOfWork)
        {
            // Invalid name
            if (String.IsNullOrEmpty((Name = Name.Trim())))
                return $"{Name} is not a valid type name";

            // If it's the update case, check if the record exists
            if (Id != null)
                if (!await unitOfWork.EquipmentTypes.isExists(q => q.Id == Id))
                    return "The type which is being updated does not exist";

            // Check if this model has already been inserted (And it's not the update case)
            if (Id == null)
                if (await unitOfWork.EquipmentTypes
                    .isExists(q => q.Name.ToLower() == Name.ToLower() && !q.IsArchieved))
                    return $"{Name} already exists";

            // Invalid parents (Check is parents exist and if the 2 level hierachy is not violated
            // (Parents don't have parents)
            if (Parents != null)
            {
                foreach (Option parent in Parents)
                {
                    var parentType = (await unitOfWork.EquipmentTypes
                        .Find<Data.Entities.EquipmentEntities.EquipmentType>(
                            where: q => q.Id == parent.Value,
                            include: q => q.Include(q => q.Parents)))
                        .FirstOrDefault();

                    if(parentType == null)
                        return $"Parent {parent.Label} not found.";

                    if(!parentType.Parents.IsNullOrEmpty())
                        return $"2 level hierarchy violation: {parent.Label} can not be a parent because " +
                            $"it is a child for another type";
                }
            }

            // Invalid properties
            if (Properties != null)
                foreach (Option property in Properties)
                    if (!await unitOfWork.Properties.isExists(q => q.Id == property.Value && !q.IsArchieved))
                        return $"Property {property.Label} not found.";

            return null;
        }
    }
}
