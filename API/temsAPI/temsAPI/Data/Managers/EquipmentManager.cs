using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Factories.LogFactories;
using temsAPI.Helpers;
using temsAPI.Services.EquipmentManagementHelpers;
using temsAPI.Helpers.Filters;
using temsAPI.Services;
using temsAPI.System_Files;
using temsAPI.ViewModels;
using temsAPI.ViewModels.Allocation;
using temsAPI.ViewModels.Equipment;
using temsAPI.Helpers.ReusableSnippets;
using FluentEmail.Core;

namespace temsAPI.Data.Managers
{
    public class EquipmentManager : EntityManager
    {
        CurrencyConvertor _currencyConvertor;
        LogManager _logManager;
        ILogger<EquipmentManager> _logger;
        IFetcher<Equipment, EquipmentFilter> _equipmentFetcher;
        IFetcher<EquipmentAllocation, AllocationFilter> _allocationFetcher;
        IEquipmentLabelManager _equipmentLabelManager;

        public EquipmentManager(
            IUnitOfWork unitOfWork, 
            ClaimsPrincipal user,
            CurrencyConvertor currencyConvertor,
            ILogger<EquipmentManager> logger,
            LogManager logManager,
            IFetcher<Equipment, EquipmentFilter> equipmentFetcher,
            IFetcher<EquipmentAllocation, AllocationFilter> allocationFetcher,
            IEquipmentLabelManager equipmentLabelManager) : base(unitOfWork, user)
        {
            _currencyConvertor = currencyConvertor;
            _logger = logger;
            _logManager = logManager;
            _equipmentFetcher = equipmentFetcher;
            _allocationFetcher = allocationFetcher;
            _equipmentLabelManager = equipmentLabelManager;
        }

        public async Task<string> Create(AddEquipmentViewModel viewModel)
        {
            string validationResult = await AddEquipmentViewModel.Validate(_unitOfWork, viewModel);
            if (validationResult != null)
                return validationResult;

            var equipment = Equipment.FromViewModel(_user, viewModel);
            await _equipmentLabelManager.SetLabel(equipment);
            
            await _unitOfWork.Equipments.Create(equipment);
            await _unitOfWork.Save();
            return null;
        }

        public async Task<string> Remove(string equipmentId)
        {
            var equipment = await GetFullEquipmentById(equipmentId);
            if (equipment == null)
                return "Invalid Id provided";

            return await Remove(equipment);
        }

        public async Task<string> Remove(Equipment equipment)
        {
            // Remove equipment children first
            var children = (await _unitOfWork.Equipments
                .FindAll<Equipment>(q => q.ParentID == equipment.Id))
                .ToList();

            foreach (Equipment eq in children)
            {
                _unitOfWork.Equipments.Delete(eq);
            }
            await _unitOfWork.Save();

            _unitOfWork.Equipments.Delete(equipment);
            await _unitOfWork.Save();
            return null;
        }

        public async Task<string> RemoveOfDefinition(string definitionId)
        {
            var equipment = (await _unitOfWork.Equipments
                .FindAll<Equipment>(
                    where: q => q.EquipmentDefinitionID == definitionId,
                    include: q => q
                    .Include(q => q.Children)))
                .ToList();

            foreach(Equipment eq in equipment)
                await Remove(eq);

            return null;
        }

        public async Task<string> RemoveAllocation(string allocationId)
        {
            var allocation = await GetFullAllocationById(allocationId);
            if (allocation == null)
                return "Invalid id provided";

            return await RemoveAllocation(allocation);
        }   

        public async Task<string> RemoveAllocation(EquipmentAllocation allocation)
        {
            _unitOfWork.EquipmentAllocations.Delete(allocation);
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

            var eqUpdatedLog = new EquipmentUpdatedLogFactory(model, IdentityService.GetUserId(_user)).Create();
            await _logManager.Create(eqUpdatedLog);

            return null;
        }

        [Obsolete]
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
                ExpressionCombiner.And(eqOfRoomsExpression, eqOfPersonnelExpression);

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
                    select: q => ViewEquipmentSimplifiedViewModel.FromModel(q.Equipment)
                    )).ToList();

            return equipment;
        }

        [Obsolete]
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
                    select: q => ViewEquipmentSimplifiedViewModel.FromModel(q)
                    )).ToList();

            return equipment;
        }

        public async Task<List<ViewEquipmentSimplifiedViewModel>> GetEquipment(EquipmentFilter filter)
        {
            var equipment = (await _equipmentFetcher.Fetch(filter))
                .Select(q => ViewEquipmentSimplifiedViewModel.FromModel(q))
                .ToList();

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

        public async Task ChangeWorkingState(Equipment equipment, bool isDefect)
        {
            equipment.IsDefect = isDefect;
            await _unitOfWork.Save();

            var workingStateChangedLog = new EquipmentWorkingStateChangedLogFactory(equipment, IdentityService.GetUserId(_user)).Create();
            await _logManager.Create(workingStateChangedLog);
        }

        public async Task ChangeUsingState(Equipment equipment, bool isUsed)
        {
            equipment.IsUsed = isUsed;

            foreach(Equipment child in equipment.Children)
            {
                child.IsUsed = isUsed;
                var childUsingChanged = new EquipmentUsingStateChangedLogFactory(child, IdentityService.GetUserId(_user)).Create();
                await _logManager.Create(childUsingChanged);
            }

            await _unitOfWork.Save();
            
            var parentUsingChanged = new EquipmentUsingStateChangedLogFactory(equipment, IdentityService.GetUserId(_user)).Create();
            await _logManager.Create(parentUsingChanged);
        }

        public async Task<string> Detach(Equipment equipment)
        {
            var parent = await GetFullEquipmentById(equipment.ParentID);

            equipment.ParentID = null;
            _equipmentLabelManager.SetLabel(equipment, EquipmentLabel.Part);
            await _unitOfWork.Save();

            string createdById = IdentityService.GetUserId(_user);
            var eqDetachedChildLog = new ChildEquipmentDetachedChildLogFactory(parent, equipment, createdById).Create();
            var eqDetachedParentLog = new ChildEquipmentDetachedParentLogFactory(parent, equipment, createdById).Create();

            await _logManager.Create(eqDetachedParentLog);
            await _logManager.Create(eqDetachedChildLog);

            return null;
        }

        public async Task Attach(Equipment parent, Equipment child)
        {
            if (child.ParentID != null)
                await Detach(child);

            _equipmentLabelManager.SetLabel(child, EquipmentLabel.Component);
            parent.Children.Add(child);
            await _unitOfWork.Save();

            string createdById = IdentityService.GetUserId(_user);
            var eqAttachedParentLog = new ChildEquipmentAttachedParentLogFactory(parent, child, createdById).Create();
            var eqAttachedChildLog = new ChildEquipmentAttachedChildLogFactory(parent, child, createdById).Create();

            await _logManager.Create(eqAttachedChildLog);
            await _logManager.Create(eqAttachedParentLog);
        }

        public async Task<List<Option>> GetEquipmentOfDefinitions(List<string> definitionIds, bool onlyParents)
        {
            Expression<Func<Equipment, bool>> expression = q =>
                    definitionIds.Contains(q.EquipmentDefinitionID) && !q.IsArchieved;

            if (onlyParents)
                expression = ExpressionCombiner.And(expression, q => q.ParentID == null);

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

        public async Task<IEnumerable<Equipment>> GetDetachedEquipment(EquipmentFilter filter)
        {
            return await _equipmentFetcher.Fetch(filter);
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

                expression = ExpressionCombiner.And(expression, expression2);
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

                    // Allocate parent along with it's children
                    var equipmentToBeAllocated = (await _unitOfWork.Equipments
                        .Find<Equipment>(
                            where: q => q.Id == equipment.Value,
                            include: q => q.Include(q => q.Children)))
                        .Select(q =>
                        {
                            // equipmentToBeAllocated = parent + children entities
                            List<Equipment> equipmentToBeAllocated = new List<Equipment>() { q };

                            if (q.Children.IsNullOrEmpty())
                                return equipmentToBeAllocated;

                            q.Children.ForEach(ch => equipmentToBeAllocated.Add(ch));
                            return equipmentToBeAllocated;
                        })
                        .FirstOrDefault();

                    string currentUserId = IdentityService.GetUserId(_user);

                    foreach(var eqToAllocate in equipmentToBeAllocated)
                    {
                        var model = new EquipmentAllocation
                        {
                            Id = Guid.NewGuid().ToString(),
                            DateAllocated = DateTime.Now,
                            EquipmentID = eqToAllocate.Id,
                            Label = eqToAllocate.Label 
                        };

                        if (viewModel.AllocateToType == "personnel")
                            model.PersonnelID = viewModel.AllocateToId;
                        else
                            model.RoomID = viewModel.AllocateToId;

                        await _unitOfWork.EquipmentAllocations.Create(model);
                        await _unitOfWork.Save();

                        var logModel = await GetFullAllocationById(model.Id);

                        var eqAllocationLog = new AllocationEquipmentLogFactory(logModel, currentUserId).Create();
                        var allocateeAllocationLog = (logModel.RoomID != null)
                            ? new AllocationRoomLogFactory(logModel, currentUserId).Create()
                            : new AllocationPersonnelLogFactory(logModel, currentUserId).Create();

                        await _logManager.Create(eqAllocationLog);
                        await _logManager.Create(allocateeAllocationLog);
                    }
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

        public async Task<string> MarkAllocationAsReturned(string allocationId)
        {
            var allocation = await GetFullAllocationById(allocationId);
            if (allocation == null)
                return "Invalid allocation provided";

            allocation.DateReturned = DateTime.Now;
            await _unitOfWork.Save();

            return null;
        }

        public async Task<string> ArchieveAllocation(string allocationId, bool archivationStatus = true)
        {
            string archivationResult = await new ArchieveHelper(_unitOfWork, _user)
                .SetEquipmenAllocationtArchivationStatus(allocationId, archivationStatus);
            if (archivationResult != null)
                return archivationResult;

            return null;
        }

        // BEFREE: Make it generic
        public double GetEquipmentPriceInLei(Equipment equipment)
        {
            switch (equipment.Currency)
            {
                case "lei":
                    return (double)equipment.Price;
                case "eur":
                    return (double)equipment.Price * _currencyConvertor.EUR_MDL_rate;
                case "usd":
                    return (double)equipment.Price * _currencyConvertor.USD_MDL_rate;
                default:
                    return 0;
            }
        }

        public async Task<List<ViewAllocationSimplifiedViewModel>> GetAllocations(AllocationFilter filter)
        {
            var allocations = (await _allocationFetcher.Fetch(filter))
                .Select(q => ViewAllocationSimplifiedViewModel.FromModel(q))
                .ToList();

;           return allocations;
        }

        public async Task<int> GetTotalItems(AllocationFilter filter)
        {
            //var filterExpression = GetFilterExpressionFromEntityCollection(entityCollection);
            var number = await _allocationFetcher.GetAmount(filter);
            return number;
        }

        public async Task<EquipmentAllocation> GetFullAllocationById(string allocationId)
        {
            var allocation = (await _unitOfWork.EquipmentAllocations
                    .Find<EquipmentAllocation>(
                        where: q => q.Id == allocationId,
                        include: q => q
                        .Include(q => q.Personnel)
                        .Include(q => q.Room)
                        .Include(q => q.Equipment)
                        .ThenInclude(q => q.EquipmentDefinition)
                        .ThenInclude(q => q.EquipmentType)))
                    .FirstOrDefault();

            return allocation;
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

        // Extract to sepparate classes
        [Obsolete("Build an instance of EquipmentFilter and use EquipmentFetcher service for this scope")]
        public Expression<Func<Equipment, bool>> Eq_FilterByAllocateeEntity(
            string entityType = null, 
            string entityId = null)
        {
            // Not interested in archived items, and those for which 
            Expression<Func<Equipment, bool>> expression = q => !q.IsArchieved && q.EquipmentDefinition.ParentID == null;

            if (entityType == null)
                return expression;

            entityType = entityType.ToLower();
            if (entityType == "equipment" || !HardCodedValues.EntityTypes.Contains(entityType) || entityId == null)
                return expression;
            
            Expression<Func<Equipment, bool>> secondaryExpression = null;

            // BEFREE: Not good at all! (store the active allocation in a var).
            switch (entityType)
            {
                case "room":
                    secondaryExpression = q => q.EquipmentAllocations.FirstOrDefault(q => q.DateReturned == null) != null
                    && q.EquipmentAllocations.FirstOrDefault(q => q.DateReturned == null).RoomID == entityId;
                    break;
                case "personnel":
                    secondaryExpression = q => q.EquipmentAllocations.FirstOrDefault(q => q.DateReturned == null) != null
                     && q.EquipmentAllocations.FirstOrDefault(q => q.DateReturned == null).PersonnelID == entityId;
                    break;
            }

            expression = ExpressionCombiner.And(expression, secondaryExpression);
            return expression;
        }
    }
}
