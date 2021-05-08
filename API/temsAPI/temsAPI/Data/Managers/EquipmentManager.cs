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
                    select: q => EquipmentToViewEquipmentSimplifiedViewModel(q.Equipment)
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
                    select: q => EquipmentToViewEquipmentSimplifiedViewModel(q)
                    )).ToList();

            return equipment;
        }

        public async Task<Equipment> GetEquipmentById(string id)
        {
            Equipment equipment = (await _unitOfWork.Equipments
                .Find<Equipment>(
                where: q => q.Id == id,
                include: q => q
                            .Include(q => q.EquipmentDefinition)
                            .ThenInclude(q => q.EquipmentType)))
                .FirstOrDefault();

            return equipment;
        }

        public ViewEquipmentSimplifiedViewModel EquipmentToViewEquipmentSimplifiedViewModel(Equipment eq)
        {
            ViewEquipmentSimplifiedViewModel viewEquipmentSimplified = new ViewEquipmentSimplifiedViewModel
            {
                Id = eq.Id,
                IsDefect = eq.IsDefect,
                IsUsed = eq.IsUsed,
                IsArchieved = eq.IsArchieved,
                TemsId = eq.TEMSID,
                SerialNumber = eq.SerialNumber,
                Type = eq.EquipmentDefinition.EquipmentType.Name,
                Definition = eq.EquipmentDefinition.Identifier,
            };

            var lastAllocation = eq.EquipmentAllocations
                .FirstOrDefault(q => !q.IsArchieved && q.DateReturned == null);
                
            if (lastAllocation == null)
                viewEquipmentSimplified.Assignee = "Deposit";
            else
                viewEquipmentSimplified.Assignee =
                    (lastAllocation.Room != null)
                    ? "Room: " + lastAllocation.Room.Identifier
                    : "Personnel: " + lastAllocation.Personnel.Name;

            viewEquipmentSimplified.TemsIdOrSerialNumber =
                String.IsNullOrEmpty(viewEquipmentSimplified.TemsId)
                ? viewEquipmentSimplified.SerialNumber
                : viewEquipmentSimplified.TemsId;

            return viewEquipmentSimplified;
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
    }
}
