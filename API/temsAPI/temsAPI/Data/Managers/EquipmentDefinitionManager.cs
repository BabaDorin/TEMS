using Microsoft.EntityFrameworkCore;
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
using temsAPI.ViewModels.EquipmentDefinition;

namespace temsAPI.Data.Managers
{
    public class DefinitionsOfTypesModel
    {
        public string Filter { get; set; }
        public List<string> TypeIds { get; set; }
    }

    public class EquipmentDefinitionManager : EntityManager
    {
        EquipmentManager _equipmentManager;

        public EquipmentDefinitionManager(
            IUnitOfWork unitOfWork, 
            ClaimsPrincipal user,
            EquipmentManager equipmentManager) : base(unitOfWork, user)
        {
            _equipmentManager = equipmentManager;
        }

        public async Task<string> Create(AddEquipmentDefinitionViewModel viewModel)
        {
            string validationResult = await AddEquipmentDefinitionViewModel.Validate(_unitOfWork, viewModel);
            if (validationResult != null)
                return validationResult;

            var equipmentDefinition = await EquipmentDefinition.FromViewModel(_unitOfWork, viewModel);

            await _unitOfWork.EquipmentDefinitions.Create(equipmentDefinition);
            await _unitOfWork.Save();

            return null;
        }

        // Remove by Id
        public async Task<string> Remove(string definitionId)
        {
            var definition = await GetFullById(definitionId);
            if (definition == null)
                return "Invalid id provided";

            return await Remove(definition);
        }

        // Remove by reference
        public async Task<string> Remove(EquipmentDefinition definition)
        {
            // Remove children definitions along with associated equipment first
            var children = definition.Children.ToList();

            foreach (EquipmentDefinition child in children)
            {
                await _equipmentManager.RemoveOfDefinition(child.Id);
                _unitOfWork.EquipmentDefinitions.Delete(child);
            }
            await _unitOfWork.Save();

            await _equipmentManager.RemoveOfDefinition(definition.Id);
            _unitOfWork.EquipmentDefinitions.Delete(definition);
            await _unitOfWork.Save();
            return null;
        }

        public async Task<string> RemoveOfType(string typeId)
        {
            var definitions = (await _unitOfWork.EquipmentDefinitions
                .FindAll<EquipmentDefinition>(
                    where: q => q.EquipmentTypeID == typeId,
                    include: q => q.Include(q => q.Children)))
                .ToList();

            foreach (EquipmentDefinition def in definitions)
            {
                await Remove(def);
            }

            return null;
        }

        public async Task<string> Update(AddEquipmentDefinitionViewModel viewModel)
        {
            string validationResult = await AddEquipmentDefinitionViewModel.Validate(_unitOfWork, viewModel);
            if (validationResult != null)
                return validationResult;

            EquipmentDefinition definition = (await _unitOfWork.EquipmentDefinitions
                .Find<EquipmentDefinition>(
                    where: q => q.Id == viewModel.Id,
                    include: q => q
                    .Include(q => q.EquipmentSpecifications)
                    .Include(q => q.Children)
                )).FirstOrDefault();

            definition.Identifier = viewModel.Identifier;
            definition.EquipmentTypeID = viewModel.TypeId;
            definition.EquipmentTypeID = viewModel.TypeId;
            definition.Price = viewModel.Price;
            definition.Currency = viewModel.Currency;
            definition.Description = viewModel.Description;

            await AssignSpecifications(definition, viewModel);
            definition.Children.Clear();
            await EquipmentDefinition.SetDefinitionChildren(_unitOfWork, definition, viewModel.Children);
            await _unitOfWork.Save();

            return null;
        }

        public async Task<List<Option>> GetOfType(string typeId)
        {
            var options = (await _unitOfWork.EquipmentDefinitions
                .FindAll<Option>(
                    where: q => q.EquipmentTypeID == typeId && !q.IsArchieved,
                    select: q => new Option
                    {
                        Value = q.Id,
                        Label = q.Identifier
                    })).ToList();

            return options;
        }

        public async Task<List<Option>> GetOfTypes(DefinitionsOfTypesModel filter, int skip = 0, int take = int.MaxValue)
        {
            Expression<Func<EquipmentDefinition, bool>> expression = q => !q.IsArchieved;

            if(filter != null)
            {
                if (filter.TypeIds == null)
                    filter.TypeIds = new List<string>();

                if (filter.TypeIds.Count > 0)
                    expression = ExpressionCombiner.CombineTwo(
                        expression,
                        q => filter.TypeIds.Contains(q.EquipmentTypeID));

                if (filter.Filter != null)
                    expression = ExpressionCombiner.CombineTwo(
                        expression,
                        q => q.Identifier.Contains(filter.Filter));
            }

            List<Option> options = (await _unitOfWork.EquipmentDefinitions
                .FindAll<Option>(
                    where: expression,
                    take: 5,
                    select: q => new Option
                    {
                        Value = q.Id,
                        Label = q.Identifier,
                        Additional = q.EquipmentTypeID
                    }
                )).ToList();

            return options;
        }

        public async Task<List<ViewEquipmentDefinitionSimplifiedViewModel>> GetSimplified()
        {
            List<ViewEquipmentDefinitionSimplifiedViewModel> defs =
                    (await _unitOfWork.EquipmentDefinitions
                    .FindAll<ViewEquipmentDefinitionSimplifiedViewModel>(
                        where: q => !q.IsArchieved,
                        include: q => q
                        .Include(q => q.Parent)
                        .Include(q => q.Children.Where(q => !q.IsArchieved))
                        .Include(q => q.EquipmentType),
                        select: q => ViewEquipmentDefinitionSimplifiedViewModel.FromModel(q)
                    )).ToList();

            return defs;
        }

        public async Task<ViewEquipmentDefinitionSimplifiedViewModel> GetSimplifiedById(string id)
        {
            ViewEquipmentDefinitionSimplifiedViewModel def =
                    (await _unitOfWork.EquipmentDefinitions
                    .Find<ViewEquipmentDefinitionSimplifiedViewModel>(
                        where: q => q.Id == id,
                        include: q => q
                        .Include(q => q.Parent)
                        .Include(q => q.Children.Where(q => !q.IsArchieved))
                        .Include(q => q.EquipmentType),
                        select: q => ViewEquipmentDefinitionSimplifiedViewModel.FromModel(q)
                    )).FirstOrDefault();

            return def;
        }

        public async Task<EquipmentDefinition> GetById(string id)
        {
            var definition = (await _unitOfWork.EquipmentDefinitions
                .Find<EquipmentDefinition>(q => q.Id == id))
                .FirstOrDefault();

            return definition;
        }

        public async Task<EquipmentDefinition> GetFullById(string id)
        {
            var definition = (await _unitOfWork.EquipmentDefinitions
                    .Find<EquipmentDefinition>(
                        where: q => q.Id == id,
                        include: q => q
                        .Include(q => q.Children.Where(q => !q.IsArchieved))
                        .ThenInclude(q => q.EquipmentType)
                        .Include(q => q.EquipmentSpecifications)
                        .ThenInclude(q => q.Property).ThenInclude(q => q.DataType)
                        .Include(q => q.Parent)
                        .Include(q => q.EquipmentType)))
                    .FirstOrDefault();

            return definition;
        }


        // Utilities

        /// <summary>
        /// Assigns values for definition's properties according to the data being provided by the
        /// DefinitionViewModel.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private async Task AssignSpecifications(EquipmentDefinition model, AddEquipmentDefinitionViewModel viewModel)
        {
            var specifications = model.EquipmentSpecifications?.ToList();
            foreach (var item in specifications)
            {
                model.EquipmentSpecifications.Remove(item);
            }

            foreach (var property in viewModel.Properties)
            {
                model.EquipmentSpecifications.Add(new EquipmentSpecifications
                {
                    Id = Guid.NewGuid().ToString(),
                    EquipmentDefinitionID = model.Id,
                    PropertyID = (await _unitOfWork.Properties.Find<Property>(q => q.Name == property.Value))
                        .FirstOrDefault().Id,
                    Value = property.Label,
                });
            }
        }
    }
}
