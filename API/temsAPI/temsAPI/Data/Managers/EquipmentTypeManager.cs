using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorLight.Extensions;
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
using temsAPI.ViewModels.EquipmentType;

namespace temsAPI.Data.Managers
{
    public class EquipmentTypeManager : EntityManager
    {
        EquipmentDefinitionManager _equipmentDefinitionManager;
        EquipmentManager _equipmentManager;

        public EquipmentTypeManager(
            IUnitOfWork unitOfWork, 
            ClaimsPrincipal user,
            EquipmentDefinitionManager equipmentDefinitionManager,
            EquipmentManager equipmentManager) : base(unitOfWork, user)
        {
            _equipmentDefinitionManager = equipmentDefinitionManager;
            _equipmentManager = equipmentManager;
        }

        public async Task<string> Create(AddEquipmentTypeViewModel viewModel)
        {
            string validationResult = await viewModel.Validate(_unitOfWork);
            if (validationResult != null)
                return validationResult;

            EquipmentType equipmentType = new EquipmentType
            {
                Id = Guid.NewGuid().ToString(),
                Name = viewModel.Name,
            };

            await SetProperties(equipmentType, viewModel);

            await _unitOfWork.EquipmentTypes.Create(equipmentType);
            await _unitOfWork.Save();

            string setParentsResult = await SetParents(equipmentType, viewModel);
            if (setParentsResult != null)
                return setParentsResult;

            return null;
        }

        public async Task<string> Remove(string typeId)
        {
            var type = (await _unitOfWork.EquipmentTypes
                .Find<EquipmentType>(
                    where: q => q.Id == typeId,
                    include: q => q
                    .Include(q => q.Children)
                    .ThenInclude(q => q.Parents)))
                .FirstOrDefault();

            if (type == null)
                return "Invalid Id provided";

            return await Remove(type);
        }

        public async Task<string> Remove(EquipmentType type)
        {
            // Remove ONLY children that are assigned to this type
            var typeChildren = type.Children.ToList();

            foreach (EquipmentType child in typeChildren)
            {
                if (child.Parents.Count > 1)
                    continue;

                await _equipmentDefinitionManager.RemoveOfType(child.Id);
                _unitOfWork.EquipmentTypes.Delete(child);
                await _unitOfWork.Save();
            }

            await _equipmentDefinitionManager.RemoveOfType(type.Id);
            _unitOfWork.EquipmentTypes.Delete(type);
            await _unitOfWork.Save();
            return null;
        }

        public async Task<string> Update(AddEquipmentTypeViewModel viewModel)
        {
            string validationResult = await viewModel.Validate(_unitOfWork);
            if (validationResult != null)
                return validationResult;

            var equipmentTypeToUpdate = await GetFullById(viewModel.Id);

            if (equipmentTypeToUpdate == null)
                return "An error occured, the type has not been found";

            if ((bool)!equipmentTypeToUpdate.EditableTypeInfo)
                return "This type can not be edited";

            await SetProperties(equipmentTypeToUpdate, viewModel);

            string setParentsResponse = await SetParents(equipmentTypeToUpdate, viewModel);
            if (setParentsResponse != null)
                return setParentsResponse;

            equipmentTypeToUpdate.Name = viewModel.Name;
            await _unitOfWork.Save();

            return null;
        }

        public async Task<List<Option>> GetAutocompleteOptions(string filter, bool includeChildTypes)
        {
            int take = (filter == null) ? int.MaxValue : 5;

            Expression<Func<EquipmentType, bool>> expression = q => !q.IsArchieved;

            if (!includeChildTypes)
                expression = expression.Concat(q => q.Parents.Count() == 0);

            if (filter != null)
                expression = expression.Concat(q => q.Name.Contains(filter));

            Func<IQueryable<EquipmentType>, IOrderedQueryable<EquipmentType>> orderBy =
                q => q.OrderBy(q => q.Name);

            List<Option> options = (await _unitOfWork.EquipmentTypes.FindAll<Option>(
                include: q => q.Include(q => q.Parents),
                where: expression,
                take: take,
                orderBy: orderBy,
                select: q => new Option
                {
                    Value = q.Id,
                    Label = q.Name,
                })).ToList();

            return options;
        }

        public async Task<List<ViewEquipmentTypeSimplifiedViewModel>> GetSimplified()
        {
            var types = (await _unitOfWork.EquipmentTypes
                .FindAll<ViewEquipmentTypeSimplifiedViewModel>(
                    where: q => q.IsArchieved == false,
                    include: q => q
                    .Include(q => q.Children.Where(q => !q.IsArchieved))
                    .Include(q => q.Parents.Where(q => !q.IsArchieved)),
                    select: q => ViewEquipmentTypeSimplifiedViewModel.FromModel(q)
                    )).ToList();

            return types;
        }

        public async Task<ViewEquipmentTypeSimplifiedViewModel> GetSimplifiedById(string typeId)
        {
            var type = (await _unitOfWork.EquipmentTypes
                .Find<ViewEquipmentTypeSimplifiedViewModel>(
                    where: q => q.Id == typeId,
                    include: q => q
                    .Include(q => q.Children.Where(q => !q.IsArchieved))
                    .Include(q => q.Parents.Where(q => !q.IsArchieved)),
                    select: q => ViewEquipmentTypeSimplifiedViewModel.FromModel(q)
                    )).FirstOrDefault();

            return type;
        }

        public async Task<EquipmentType> GetById(string typeId)
        {
            var type = (await _unitOfWork.EquipmentTypes
                .Find<EquipmentType>(q => q.Id == typeId))
                .FirstOrDefault();

            return type;
        }

        public async Task<EquipmentType> GetFullById(string typeId)
        {
            var type = (await _unitOfWork.EquipmentTypes
                .Find<EquipmentType>(
                    where: q => q.Id == typeId,
                    include: q => q
                    .Include(q => q.Parents.Where(q => !q.IsArchieved))
                    .Include(q => q.Properties.Where(q => !q.IsArchieved))
                    .ThenInclude(q => q.DataType)
                    .Include(q => q.Children.Where(q => !q.IsArchieved)).ThenInclude(q => q.Parents)
                    .Include(q => q.Children.Where(q => !q.IsArchieved)).ThenInclude(q => q.Properties.Where(q => !q.IsArchieved))
                    .ThenInclude(q => q.DataType)
                    )).FirstOrDefault();

            return type;
        }

        public async Task<List<Option>> GetPropertiesOfType(string typeId)
        {
            List<Option> props = (await _unitOfWork.EquipmentTypes
                .Find<List<Option>>(
                    include: q => q.Include(q => q.Properties),
                    where: q => q.Id == typeId,
                    select: q => q.Properties.Select(q => new Option
                    {
                        Value = q.Id,
                        Label = q.DisplayName,
                        Additional = q.Name
                    }).ToList()
                )).FirstOrDefault();

            return props;
        }

        // Utilities => To be moved to another file

        /// <summary>
        /// Sets properties from view model to the model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private async Task SetProperties(EquipmentType model, AddEquipmentTypeViewModel viewModel)
        {
            if (model.Properties.Select(q => q.Id).SequenceEqual(viewModel.Properties.Select(q => q.Value)))
                return;

            var modelProperties = model.Properties.ToList();
            foreach (var item in modelProperties)
            {
                model.Properties.Remove(item);
            }
            await _unitOfWork.Save();

            if (viewModel.Properties != null)
                foreach (Option prop in viewModel.Properties)
                {
                    model.Properties.Add((await _unitOfWork.Properties.Find<Property>(q => q.Id == prop.Value && !q.IsArchieved))
                            .FirstOrDefault());

                    await _unitOfWork.Save();
                }
        }

        /// <summary>
        /// Sets the parents from view model to the model. Returns null if success, otherwise - 
        /// returns the issue.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private async Task<string> SetParents(EquipmentType model, AddEquipmentTypeViewModel viewModel)
        {
            if (viewModel.Parents == null || viewModel.Parents.Count == 0)
                return null;

            if (model.Parents.Select(q => q.Id).SequenceEqual(viewModel.Parents.Select(q => q.Value)))
                return null;

            var modelParents = model.Parents.ToList();
            foreach (var item in modelParents)
            {
                model.Parents.Remove(item);
            }
            await _unitOfWork.Save();

            if (viewModel.Parents != null)
                foreach (Option parent in viewModel.Parents)
                {
                    var parentType = (await _unitOfWork.EquipmentTypes
                        .Find<EquipmentType>(q => q.Id == parent.Value))
                        .FirstOrDefault();

                    if (parentType == null || parentType.Id == model.Id)
                        return "One or more parents are invalid";

                    model.Parents.Add(parentType);
                    //parentType.Children.Add(model);
                    await _unitOfWork.Save();
                }

            return null;
        }
    }
}
