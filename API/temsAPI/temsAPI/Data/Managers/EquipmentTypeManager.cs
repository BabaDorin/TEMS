﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.ViewModels;
using temsAPI.ViewModels.EquipmentType;

namespace temsAPI.Data.Managers
{
    public class EquipmentTypeManager : EntityManager
    {
        public EquipmentTypeManager(IUnitOfWork unitOfWork, ClaimsPrincipal user) : base(unitOfWork, user)
        {
        }

        public async Task<List<Option>> GetAutocompleteOptions(string filter)
        {
            int take = (filter == null) ? int.MaxValue : 5;
            Expression<Func<EquipmentType, bool>> expression = (filter == null)
               ? q => !q.IsArchieved
               : q => !q.IsArchieved && q.Name.Contains(filter);

            List<Option> options = (await _unitOfWork.EquipmentTypes.FindAll<Option>(
                where: expression,
                take: take,
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
                    include: q => q.Include(q => q.Children.Where(q => !q.IsArchieved)),
                    select: q => new ViewEquipmentTypeSimplifiedViewModel
                    {
                        Id = q.Id,
                        Name = q.Name,
                        Editable = (bool)q.EditableTypeInfo,
                        Children = String.Join(", ", q.Children
                        .Where(q => !q.IsArchieved)
                        .Select(q => q.Name))
                    }
                    )).ToList();

            return types;
        }

        public async Task<ViewEquipmentTypeSimplifiedViewModel> GetSimplifiedById(string typeId)
        {
            var type = (await _unitOfWork.EquipmentTypes
                .Find<ViewEquipmentTypeSimplifiedViewModel>(
                    where: q => q.Id == typeId,
                    include: q => q.Include(q => q.Children.Where(q => !q.IsArchieved)),
                    select: q => new ViewEquipmentTypeSimplifiedViewModel
                    {
                        Id = q.Id,
                        Name = q.Name,
                        Editable = (bool)q.EditableTypeInfo,
                        Children = String.Join(", ", q.Children.Select(q => q.Name))
                    }
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
                    .Include(q => q.Children.Where(q => !q.IsArchieved))
                    .ThenInclude(q => q.Properties.Where(q => !q.IsArchieved))
                    .ThenInclude(q => q.DataType)
                    )).FirstOrDefault();

            return type;
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

        // Utilities

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