using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OfficeOpenXml.FormulaParsing.ExpressionGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Helpers;
using temsAPI.ViewModels;
using temsAPI.ViewModels.Property;
using DataType = temsAPI.Data.Entities.EquipmentEntities.DataType;

namespace temsAPI.Data.Managers
{
    public class EquipmentPropertyManager : EntityManager
    {
        public EquipmentPropertyManager(IUnitOfWork unitOfWork, ClaimsPrincipal user) : base(unitOfWork, user)
        {
        }

        public async Task<List<Option>> GetAutocompleteOptions(string filter = null)
        {
            Expression<Func<Data.Entities.EquipmentEntities.Property, bool>> expression = q => !q.IsArchieved;
            int take = int.MaxValue;

            if(filter != null)
            {
                take = 5;
                expression = ExpressionCombiner.CombineTwo(
                    expression, q => q.DisplayName.Contains(filter));
            }

            var options = (await _unitOfWork.Properties
                .FindAll<Option>(
                    where: expression,
                    take: take,
                    select: q => new Option
                    {
                        Value = q.Id,
                        Label = q.DisplayName
                    }
                )).ToList();

            return options;
        }

        public async Task<List<ViewPropertySimplifiedViewModel>> GetSimplified()
        {
            var properties = (await _unitOfWork.Properties
                .FindAll(
                    where: q => !q.IsArchieved,
                    select: q => new ViewPropertySimplifiedViewModel
                    {
                        Id = q.Id,
                        Description = q.Description,
                        DisplayName = q.DisplayName,
                        Editable = q.EditablePropertyInfo
                    }
                )).ToList();

            return properties;
        }

        public async Task<ViewPropertySimplifiedViewModel> GetSimplifiedById(string propertyId)
        {
            var property = (await _unitOfWork.Properties
                .Find(
                where: q => q.Id == propertyId,
                select: q => new ViewPropertySimplifiedViewModel
                {
                    Id = q.Id,
                    Description = q.Description,
                    DisplayName = q.DisplayName,
                    Editable = (bool)q.EditablePropertyInfo
                }
                )).FirstOrDefault();

            return property;
        }

        public async Task<Entities.EquipmentEntities.Property> GetById(string propertyId)
        {
            var property = (await _unitOfWork.Properties
                .Find<Data.Entities.EquipmentEntities.Property>(q => q.Id == propertyId))
                .FirstOrDefault();

            return property;
        }

        public async Task<Entities.EquipmentEntities.Property> GetFullById(string propertyId)
        {
            var property = (await _unitOfWork.Properties
                .Find<Data.Entities.EquipmentEntities.Property>(
                 where: q => q.Id == propertyId,
                 include: q => q.Include(q => q.DataType)))
                .FirstOrDefault();

            return property;
        }

        public async Task<string> Create(AddPropertyViewModel viewModel)
        {
            string validationResult = await viewModel.Validate(_unitOfWork);
            if (validationResult != null)
                return validationResult;

            // If we got so far, it might be valid.
            var property = new Entities.EquipmentEntities.Property()
            {
                Id = Guid.NewGuid().ToString(),
                Name = viewModel.Name,
                DisplayName = viewModel.DisplayName,
                Description = viewModel.Description,
                Required = viewModel.Required,
                DataType = (await _unitOfWork.DataTypes.Find<DataType>(q => q.Name.ToLower() == viewModel.DataType))
                    .FirstOrDefault(),
            };

            await _unitOfWork.Properties.Create(property);
            await _unitOfWork.Save();

            return null;
        }

        public async Task<string> Remove(string propertyId)
        {
            var property = await GetFullById(propertyId);
            if (property == null)
                return "Invalid Id provided";

            _unitOfWork.Properties.Delete(property);
            await _unitOfWork.Save();
            return null;
        }

        public async Task<String> Update(AddPropertyViewModel viewModel)
        {
            string validationResult = await viewModel.Validate(_unitOfWork);
            if (validationResult != null)
                return validationResult;

            var propertyToUpdate = (await _unitOfWork.Properties
                .Find<Entities.EquipmentEntities.Property>(
                    where: q => q.Id == viewModel.Id,
                    include: q => q.Include(q => q.DataType)
                )).FirstOrDefault();

            if ((bool)!propertyToUpdate.EditablePropertyInfo)
                return "This property can not be edited.";

            propertyToUpdate.Name = viewModel.Name;
            propertyToUpdate.DisplayName = viewModel.DisplayName;
            propertyToUpdate.Description = viewModel.Description;

            // Update data type.
            // We check all of property specifications to ensure that the newly setted datatype
            // is compatible with specification values (Example: 'RX-11' can not be converted to Double).

            var newDataType = (await _unitOfWork.DataTypes.
                Find<DataType>(q => q.Name.ToLower() == viewModel.DataType.ToLower()))
                .FirstOrDefault();

            if(newDataType != propertyToUpdate.DataType)
            {
                var propertySpecifications = (await _unitOfWork.EquipmentSpecifications
                    .FindAll<EquipmentSpecifications>(
                        where: q => q.PropertyID == propertyToUpdate.Id
                    ))
                    .ToList();

                foreach(EquipmentSpecifications spec in propertySpecifications)
                    if (!newDataType.TryParseValue(spec.Value))
                        return $"Unable to change property datatype. Value: {spec.Value} can not be converted to new datatype: {newDataType.Name}";

                propertyToUpdate.DataType = newDataType;
            }
            
            propertyToUpdate.Required = viewModel.Required;
            await _unitOfWork.Save();

            return null;
        }
    }
}
