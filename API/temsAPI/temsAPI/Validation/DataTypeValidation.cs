using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Repository;
using temsAPI.ViewModels;

namespace temsAPI.Validation
{
    public class DataTypeValidation
    {
        /// <summary>
        /// Validates equipment specific properties
        /// </summary>
        /// <param name="vmProperty">Option instance representing the property to be validated</param>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        public static async Task<bool> IsValidAsync(Option vmProperty, IUnitOfWork unitOfWork)
        {
            Property fullProp = (await unitOfWork.Properties.Find<Property>(q => q.Name == vmProperty.Value,
                    include: q => q.Include(q => q.DataType))).FirstOrDefault();

            if (fullProp == null)
                return false;

            // if it's required:
            if(String.IsNullOrEmpty(vmProperty.Label = vmProperty.Label.Trim()) &&
                fullProp.Required){
                return false;
            }

            switch (fullProp.DataType.Name)
            {
                case "text":
                    return true;

                case "number":
                    double auxNumber;
                    if (!double.TryParse(vmProperty.Label, out auxNumber)) return false;
                    if (fullProp.Min < fullProp.Max)
                        return auxNumber >= fullProp.Min && auxNumber <= fullProp.Max;
                    return true;

                case "bool":
                    bool auxBool = false;
                    return bool.TryParse(vmProperty.Label,out auxBool);
            }

            return true;
        }
    }
}
