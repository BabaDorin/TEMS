using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Migrations;
using temsAPI.ViewModels;
using temsAPI.ViewModels.Personnel;

namespace temsAPI.Data.Managers
{
    public class PersonnelManager : EntityManager
    {
        public PersonnelManager(IUnitOfWork unitOfWork, ClaimsPrincipal user) : base(unitOfWork, user)
        {
        }

        public async Task<List<Option>> GetPositionOptions()
        {
            var positions = (await _unitOfWork.PersonnelPositions.FindAll<Option>(
                    where: q => !q.IsArchieved,
                    select: q => new Option
                    {
                        Value = q.Id,
                        Label = q.Name
                    }
                    )).ToList();

            return positions;
        }

        public async Task ConnectiWithUser(Personnel personnel, TEMSUser user)
        {
            user.Personnel = personnel;
            await _unitOfWork.Save();
        }

        public async Task<Personnel> GetById(string personnelId)
        {
            var personnel = (await _unitOfWork.Personnel
                .Find<Personnel>(
                    where: q => q.Id == personnelId,
                    include: q => q
                    .Include(q => q.Positions)))
                .FirstOrDefault();

            return personnel;
        }
        
        public async Task<string> Update(AddPersonnelViewModel viewModel)
        {
            string validationResult = await viewModel.Validate(_unitOfWork);
            if (validationResult != null)
                return validationResult;

            var personnel = (await _unitOfWork.Personnel
                 .Find<Personnel>(
                 where: q => q.Id == viewModel.Id,
                 include: q => q.Include(q => q.Positions)
                )).FirstOrDefault();

            personnel.Name = viewModel.Name;
            personnel.Email = viewModel.Email;
            personnel.PhoneNumber = viewModel.PhoneNumber;

            personnel.Positions.Clear();
            List<string> positionIds = viewModel.Positions.Select(q => q.Value).ToList();
            personnel.Positions = (await _unitOfWork.PersonnelPositions
                .FindAll<PersonnelPosition>
                (
                    where: q => positionIds.Contains(q.Id)
                )).ToList();

            await _unitOfWork.Save();

            return null;
        }

        public async Task<string> Create(AddPersonnelViewModel viewModel)
        {
            string validationResult = await viewModel.Validate(_unitOfWork);
            if (validationResult != null)
                return validationResult;

            List<string> positions = viewModel.Positions.Select(q => q.Value).ToList();
            Personnel model = new Personnel
            {
                Id = Guid.NewGuid().ToString(),
                Name = viewModel.Name,
                Email = viewModel.Email,
                Positions = await _unitOfWork.PersonnelPositions.FindAll<PersonnelPosition>(
                    where: q => positions.Contains(q.Id))
            };

            await _unitOfWork.Personnel.Create(model);
            await _unitOfWork.Save();

            return null;
        }

        public async Task<List<ViewPersonnelSimplifiedViewModel>> GetSimplified(
            int skip = 0,
            int take = int.MaxValue)
        {
            var personnel = (await _unitOfWork.Personnel
                .FindAll<ViewPersonnelSimplifiedViewModel>(
                    where: q => !q.IsArchieved,
                    include: q => q
                    .Include(q => q.EquipmentAllocations)
                    .Include(q => q.Positions)
                    .Include(q => q.Tickets),
                    select: q => ViewPersonnelSimplifiedViewModel.FromModel(q)))
                .ToList();

            return personnel;
        }

        public async Task<List<Option>> GetAutocompleteOptions(string filter)
        {
            int take = int.MaxValue;
            Expression<Func<Personnel, bool>> expression = q => !q.IsArchieved;
            if(filter != null)
            {
                expression = q => !q.IsArchieved && q.Name.Contains(filter);
                take = 5;
            }

            var options = (await _unitOfWork.Personnel
                .FindAll<Option>(
                    where: expression,
                    include: q => q.Include(q => q.Positions),
                    take: take,
                    orderBy: q => q.OrderBy(q => q.Name),
                    select: q => new Option
                    {
                        Value = q.Id,
                        Label = q.Name,
                        Additional = string.Join(", ", q.Positions.Select(q => q.Name))
                    })).ToList();

            return options;
        }

        public async Task<Personnel> GetFullById(string personnelId)
        {
            var personnel = (await _unitOfWork.Personnel
                .Find<Personnel>(
                    where: q => q.Id == personnelId,
                    include: q => q
                    .Include(q => q.Logs)
                    .Include(q => q.RoomsSupervisoried)
                    .Include(q => q.Positions)
                    .Include(q => q.EquipmentAllocations)
                    .Include(q => q.KeyAllocations)))
                .FirstOrDefault();

            return personnel;
        }
    }
}
