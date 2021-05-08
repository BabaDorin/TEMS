using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Helpers;
using temsAPI.ViewModels;
using temsAPI.ViewModels.Allocation;
using temsAPI.ViewModels.Equipment;

namespace temsAPI.Data.Managers
{
    public class EquipmentManager : EntityManager
    {
        public EquipmentManager(IUnitOfWork unitOfWork, ClaimsPrincipal user) : base(unitOfWork, user)
        {
        }

        public async Task<string> Create(AddEquipmentViewModel viewModel)
        {
            string validationResult = await AddEquipmentViewModel.Validate(_unitOfWork, viewModel);
            if (validationResult != null)
                return validationResult;

            // If we got so far, it might be valid
            var equipment = Equipment.FromViewModel(_user, viewModel);
            await _unitOfWork.Equipments.Create(equipment);
            await _unitOfWork.Save();

            return null;
        }

        public async Task<string> Update(AddEquipmentViewModel viewModel)
        {
            string validationResult = await AddEquipmentViewModel.Validate(_unitOfWork, viewModel);
            if (validationResult != null)
                return validationResult;

            var model = (await _unitOfWork.Equipments
                .Find<Equipment>(q => q.Id == viewModel.Id))
                .FirstOrDefault();

            model.Currency = viewModel.Currency;
            model.Description = viewModel.Description;
            model.IsDefect = viewModel.IsDefect;
            model.IsUsed = viewModel.IsUsed;
            model.Price = viewModel.Price;
            model.PurchaseDate = viewModel.PurchaseDate;
            model.SerialNumber = viewModel.SerialNumber;
            model.TEMSID = viewModel.Temsid;

            await _unitOfWork.Save();

            return null;
        }

        public async Task<List<ViewEquipmentSimplifiedViewModel>> GetEquipmentOfEntities(
            List<string> rooms,
            List<string> personnel,
            int skip = 0, int take = int.MaxValue)
        {
            Expression<Func<EquipmentAllocation, bool>> eqOfRoomsExpression = null;
            if (rooms.Count > 0)
                eqOfRoomsExpression = q => q.DateReturned == null && rooms.Contains(q.RoomID);

            Expression<Func<EquipmentAllocation, bool>> eqOfPersonnelExpression = null;
            if (personnel.Count > 0)
                eqOfPersonnelExpression = q => q.DateReturned == null && personnel.Contains(q.PersonnelID);

            Expression<Func<EquipmentAllocation, bool>> finalExpression =
                ExpressionCombiner.CombineTwo(eqOfRoomsExpression, eqOfPersonnelExpression);

            List<ViewEquipmentSimplifiedViewModel> equipment = (await _unitOfWork.EquipmentAllocations
                .FindAll<ViewEquipmentSimplifiedViewModel>(
                    where: finalExpression,
                    skip: skip,
                    take: take,
                    include: q => q.Include(q => q.Equipment)
                    .ThenInclude(q => q.EquipmentDefinition)
                    .ThenInclude(q => q.EquipmentType)
                    .Include(q => q.Room)
                    .Include(q => q.Personnel),
                    select: q => ViewEquipmentSimplifiedViewModel.FromEquipment(q.Equipment)
                    )).ToList();

            return equipment;
        }

        public async Task<List<ViewEquipmentSimplifiedViewModel>> GetEquipment(
            bool onlyParents,
            int skip = 0,
            int take = int.MaxValue)
        {
            Expression<Func<Equipment, bool>> expression
                    = (onlyParents) ? qu => qu.ParentID == null && !qu.IsArchieved : qu => !qu.IsArchieved;

            var equipment = (await _unitOfWork.Equipments
                .FindAll<ViewEquipmentSimplifiedViewModel>(
                    where: expression,
                    skip: skip,
                    take: take,
                    include: q => q
                    .Include(q => q.EquipmentAllocations.Where(q => q.DateReturned == null))
                    .ThenInclude(q => q.Room)
                    .Include(q => q.EquipmentAllocations.Where(q => q.DateReturned == null))
                    .ThenInclude(q => q.Personnel)
                    .Include(q => q.EquipmentDefinition)
                    .ThenInclude(q => q.EquipmentType),
                    select: q => ViewEquipmentSimplifiedViewModel.FromEquipment(q)
                    )).ToList();

            return equipment;
        }

        public async Task<Equipment> GetFullEquipmentById(string id)
        {
            var equipment = (await _unitOfWork.Equipments
                    .Find<Equipment>(
                        where: q => q.Id == id,
                        include: q => q
                        .Include(q => q.EquipmentDefinition)
                        .ThenInclude(q => q.Children)
                        .Include(q => q.EquipmentDefinition)
                        .ThenInclude(q => q.EquipmentSpecifications.Where(q => !q.IsArchieved))?
                        .ThenInclude(q => q.Property)
                        .Include(q => q.EquipmentDefinition)
                        .ThenInclude(q => q.EquipmentType)
                        .Include(q => q.EquipmentAllocations)
                        .ThenInclude(q => q.Room)
                        .Include(q => q.EquipmentAllocations)
                        .ThenInclude(q => q.Personnel)
                        .Include(q => q.Children)
                        .ThenInclude(q => q.EquipmentDefinition)
                        .ThenInclude(q => q.EquipmentType)
                        .ThenInclude(q => q.Properties.Where(q => !q.IsArchieved))
                        .Include(q => q.Parent)
                        .ThenInclude(q => q.EquipmentDefinition)
                      )).FirstOrDefault();

            return equipment;
        }

        public async Task<Equipment> GetById(string id)
        {
            var equipment = (await _unitOfWork.Equipments
                .Find<Equipment>(q => q.Id == id))
                .FirstOrDefault();

            return equipment;
        }

        public void Attach(Equipment parent, Equipment child)
        {
            parent.Children.Add(child);
        }

        public async Task ChangeWorkingState(Equipment equipment, bool isWorking)
        {
            equipment.IsDefect = !isWorking;
            await _unitOfWork.Save();
        }

        public async Task ChangeUsingState(Equipment equipment, bool isUsed)
        {
            equipment.IsUsed = isUsed;
            await _unitOfWork.Save();
        }

        public async Task<string> DetachEquipment(Equipment equipment)
        {
            equipment.ParentID = null;
            await _unitOfWork.Save();

            return null;
        }

        public async Task<List<Option>> GetEquipmentOfDefinitions(List<string> definitionIds, bool onlyParents)
        {
            Expression<Func<Equipment, bool>> expression = q =>
                    definitionIds.Contains(q.EquipmentDefinitionID) && !q.IsArchieved;

            if (onlyParents)
                expression = ExpressionCombiner.CombineTwo(expression, q => q.ParentID == null);

            var equipment = (await _unitOfWork.Equipments
                .Find<Option>(
                    include: q => q.Include(q => q.EquipmentDefinition),
                    where: expression,
                    select: q => new Option
                    {
                        Value = q.Id,
                        Label = q.Identifier,
                        Additional = $"{q.Description} {q.EquipmentDefinition.Identifier}"
                    }
                )).ToList();

            return equipment;
        }

        public async Task<List<Option>> GetAutocompleteOptions(bool onlyParents, string filter)
        {
            Expression<Func<Equipment, bool>> expression =
                   (onlyParents)
                   ? qu => qu.ParentID == null && !qu.IsArchieved
                   : qu => !qu.IsArchieved;

            if (filter != null)
            {
                Expression<Func<Equipment, bool>> expression2 =
                    q => q.TEMSID.Contains(filter);

                expression = ExpressionCombiner.CombineTwo(expression, expression2);
            }

            List<Option> autocompleteOptions = new List<Option>();

            var filteredEquipment = (await _unitOfWork.Equipments
                .FindAll<Option>(
                    where: expression,
                    include: q => q.Include(q => q.EquipmentDefinition),
                    take: 5,
                    select: q => new Option
                    {
                        Value = q.Id,
                        Label = q.TemsIdOrSerialNumber,
                        Additional = q.EquipmentDefinition.Identifier
                    }
                 ))
                .ToList();

            return filteredEquipment;
        }

        public async Task<string> CreateAllocation(AddAllocationViewModel viewModel)
        {
            string validationResult = await viewModel.Validate(_unitOfWork);
            if (validationResult != null)
                return validationResult;

            if(viewModel.AllocateToType == "personnel")
                if (!await _unitOfWork.Personnel.isExists(q => q.Id == viewModel.AllocateToId))
                    return "Allocatee id seems invalid.";

            if (viewModel.AllocateToType == "room")
                if (!await _unitOfWork.Rooms.isExists(q => q.Id == viewModel.AllocateToId))
                    return "Allocatee id seems invalid.";

            List<string> equipmentsWhereFailed = new List<string>();

            foreach (Option equipment in viewModel.Equipments)
            {
                try
                {
                    await ClosePreviousAllocations(equipment.Value);

                    var model = new EquipmentAllocation
                    {
                        Id = Guid.NewGuid().ToString(),
                        DateAllocated = DateTime.Now,
                        EquipmentID = equipment.Value,
                    };

                    if (viewModel.AllocateToType == "personnel")
                        model.PersonnelID = viewModel.AllocateToId;
                    else
                        model.RoomID = viewModel.AllocateToId;

                    await _unitOfWork.EquipmentAllocations.Create(model);
                    await _unitOfWork.Save();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    equipmentsWhereFailed.Add(equipment.Label);
                }
            }

            if (equipmentsWhereFailed.Count > 0)
                return "The following equipments have not been allocated due to unhandled error:"
                        + string.Join(",", equipmentsWhereFailed);

            return null;
        }

        private async Task ClosePreviousAllocations(string equipmentId)
        {
            if (await _unitOfWork.EquipmentAllocations
                .isExists(q => q.EquipmentID == equipmentId && q.DateReturned == null))
                (await _unitOfWork.EquipmentAllocations
                    .FindAll<EquipmentAllocation>(q => q.EquipmentID == equipmentId))
                    .ToList()
                    .ForEach(q => q.DateReturned = DateTime.Now);

            await _unitOfWork.Save();
        }
    }
}
