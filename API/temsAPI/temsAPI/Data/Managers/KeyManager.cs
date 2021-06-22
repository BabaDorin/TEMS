using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.KeyEntities;
using temsAPI.Helpers;
using temsAPI.ViewModels;
using temsAPI.ViewModels.Allocation;
using temsAPI.ViewModels.Key;

namespace temsAPI.Data.Managers
{
    public class KeyManager : EntityManager
    {
        public KeyManager(IUnitOfWork unitOfWork, ClaimsPrincipal user) : base(unitOfWork, user)
        {
        }

        public async Task<List<ViewKeySimplifiedViewModel>> GetKeysSimplified(int skip = 0, int take = int.MaxValue)
        {
            var keys = (await _unitOfWork.Keys
                .FindAll<ViewKeySimplifiedViewModel>(
                    where: q => !q.IsArchieved,
                    include: q => q
                    .Include(q => q.Room)
                    .Include(q => q.KeyAllocations)
                    .ThenInclude(q => q.Personnel),
                    select: q => ViewKeySimplifiedViewModel.FromModel(q)
                    )).ToList();

            return keys;
        }

        public async Task<List<Option>> GetAutocompleteOptions()
        {
            var options = (await _unitOfWork.Keys
                .FindAll<Option>(
                    where: q => !q.IsArchieved,
                    include: q => q.Include(q => q.Room),
                    orderBy: q => q.OrderBy(q => q.Identifier),
                    select: q => new Option
                    {
                        Value = q.Id,
                        Label = q.Identifier,
                        Additional = (q.RoomId != null) ? q.RoomId : "--"
                    })).ToList();

            return options;
        }

        public async Task<string> Create(AddKeyViewModel viewModel)
        {
            string validationResult = await viewModel.Validate(_unitOfWork);
            if (validationResult != null)
                return validationResult;

            for (int i = 0; i < viewModel.NumberOfCopies; i++)
            {
                Key model = Key.FromAddKeyViewModel(viewModel);
                await _unitOfWork.Keys.Create(model);
                await _unitOfWork.Save();
            }

            return null;
        }

        public async Task<string> Remove(string keyId)
        {
            var key = await GetFullById(keyId);
            if (key == null)
                return "Invalid id provided";

            _unitOfWork.Keys.Delete(key);
            await _unitOfWork.Save();
            return null;
        }

        public async Task<Key> GetFullById(string keyId)
        {
            return (await _unitOfWork.Keys
                .Find<Key>(
                    where: q => q.Id == keyId,
                    include: q => q.Include(q => q.Room)))
                .FirstOrDefault();
        }

        public async Task<string> CreateAllocation(AddKeyAllocation allocation)
        {
            string validationResult = await allocation.Validate(_unitOfWork);
            if (validationResult != null)
                return validationResult;

            foreach (string keyId in allocation.KeyIds)
            {
                Key key = (await _unitOfWork.Keys
                    .Find<Key>(
                        where: q => q.Id == keyId,
                        include: q => q.Include(q => q.KeyAllocations))
                    ).FirstOrDefault();

                if (key == null)
                    continue;

                // Close previous opened allocation
                key.KeyAllocations
                    .Where(q => q.DateReturned == null)
                    .ToList()
                    .ForEach(q => q.DateReturned = DateTime.Now);

                // New allocation is created
                KeyAllocation keyAllocation = new KeyAllocation
                {
                    Id = Guid.NewGuid().ToString(),
                    KeyID = keyId,
                    PersonnelID = allocation.PersonnelId,
                    DateAllocated = DateTime.Now,
                };

                await _unitOfWork.KeyAllocations.Create(keyAllocation);
                await _unitOfWork.Save();
            }

            return null;
        }

        public async Task<List<ViewKeyAllocationViewModel>> GetAllocations(
            string keyId,
            string roomId,
            string personnelId)
        {
            // Invalid keyId provided
            if (keyId != "any" && !await _unitOfWork.Keys.isExists(q => q.Id == keyId))
                throw new Exception("Invalid key provided");

            // Invalid roomId provided
            if (roomId != "any" && !await _unitOfWork.Rooms.isExists(q => q.Id == roomId))
                throw new Exception("Invalid room provided");

            // Invalid personnelId provided
            if (personnelId != "any" && !await _unitOfWork.Personnel.isExists(q => q.Id == personnelId))
                throw new Exception("Invalid personnel provied");

            Expression<Func<KeyAllocation, bool>> keyExpression = (keyId == "any")
                ? null
                : q => q.KeyID == keyId;

            Expression<Func<KeyAllocation, bool>> roomExpression = (roomId == "any")
               ? null
               : q => q.Key.RoomId == roomId;

            Expression<Func<KeyAllocation, bool>> personnelExpression = (personnelId == "any")
               ? null
               : q => q.PersonnelID == personnelId;

            var finalExpression = ExpressionCombiner.CombineTwo(keyExpression,
                            ExpressionCombiner.CombineTwo(roomExpression, personnelExpression));

            var allocations = (await _unitOfWork.KeyAllocations
                .FindAll<ViewKeyAllocationViewModel>(
                    where: finalExpression,
                    include: q => q.Include(q => q.Personnel)
                                   .Include(q => q.Key).ThenInclude(q => q.Room),
                    orderBy: q => q.OrderByDescending(q => q.DateAllocated),
                    select: q => ViewKeyAllocationViewModel.FromModel(q)
                    )).ToList();

            return allocations;
        }

        public async Task<string> MarkKeyAsReturned(string keyId)
        {
            if (keyId == null || !await _unitOfWork.Keys.isExists(q => q.Id == keyId))
                return "No id has been provided";

            var key = (await _unitOfWork.Keys
                .Find<Key>(
                    where: q => q.Id == keyId,
                    include: q => q.Include(q => q.KeyAllocations.Where(q => q.DateReturned == null))))
                    .FirstOrDefault();

            key.KeyAllocations.ToList().ForEach(q => q.DateReturned = DateTime.Now);
            await _unitOfWork.Save();

            return null;
        }
    }
}
