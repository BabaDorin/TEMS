using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Validation;

namespace temsAPI.ViewModels.Property
{
    public class AddPropertyViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string DataType { get; set; }
        public bool Required { get; set; } = false;
#nullable enable
        public string? Description { get; set; }
#nullable disable

        public async Task<string> Validate(IUnitOfWork unitOfWork)
        {
            Name = Name?.Trim();

            // name is null
            if (String.IsNullOrEmpty(Name))
                return "'Name' is required.";

            // name - no spaces or special chars
            if (Name.Any(Char.IsWhiteSpace))
                return "'Name' value can not contain spaces.";

            if (!RegexValidation.OnlyAlphaNumeric.IsMatch(Name))
                return "'Name' value can not contain non-alphanumeric characters," +
                    " Allowed: only a-z, A-Z, 0-9.";

            // displayName is null
            if (string.IsNullOrEmpty(DisplayName.Trim()))
                return "'DisplayName' is required.";

            // dataType should be a valid one
            DataType = DataType?.ToLower().Trim();
            if (!await unitOfWork.DataTypes.isExists(q => q.Name == DataType))
                return "The datatype that has been provided seems invalid.";

            Description = Description?.Trim();
            
            // If it's the update case
            if (Id != null)
                if (!await unitOfWork.Properties.isExists(q => q.Id == Id))
                    return "The property that is being updated does not exist.";

            // Check if the property already exists (and it's not the update case)
            if (Id == null)
                if (await unitOfWork.Properties
                    .isExists(q => (q.Name.ToLower() == Name.ToLower()
                                    || q.DisplayName.ToLower() == DisplayName.ToLower())
                                    && !q.IsArchieved))
                    return "This property already exists";

            return null;
        }
    }
}
