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
//using temsAPI.Migrations;
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

        public async Task<Personnel> GetById(string personnelId)
        {
            var personnel = (await _unitOfWork.Personnel
                .Find<Personnel>(
                    where: q => q.Id == personnelId,
                    include: q => q
                    .Include(q => q.Positions)
                    .Include(q => q.TEMSUser)))
                .FirstOrDefault();

            return personnel;
        }
        
        public async Task<string> Update(AddPersonnelViewModel viewModel)
        {
            string validationResult = await viewModel.Validate(_unitOfWork);
            if (validationResult != null)
                return validationResult;

            var personnel = await GetById(viewModel.Id);

            personnel.Name = viewModel.Name;
            personnel.Email = viewModel.Email;
            personnel.PhoneNumber = viewModel.PhoneNumber;

            List<string> positionIds = viewModel.Positions.Select(q => q.Value).ToList();
            await personnel.AssignPositions(positionIds, _unitOfWork);

            if (viewModel.User != null)
                await personnel.AssignUser(viewModel.User.Value, _unitOfWork);
            else
                personnel.CancelUserConnection();

            await _unitOfWork.Save();
            return null;
        }

        public async Task<string> Create(AddPersonnelViewModel viewModel)
        {
            string validationResult = await viewModel.Validate(_unitOfWork);
            if (validationResult != null)
                return validationResult;

            Personnel model = new Personnel
            {
                Id = Guid.NewGuid().ToString(),
                Name = viewModel.Name,
                Email = viewModel.Email,
            };

            List<string> positionIds = viewModel.Positions.Select(q => q.Value).ToList();
            await model.AssignPositions(positionIds, _unitOfWork);

            if (viewModel.User != null)
                await model.AssignUser(viewModel.User.Value, _unitOfWork);

            await _unitOfWork.Personnel.Create(model);
            await _unitOfWork.Save();

            return null;
        }

        public async Task<string> Remove(string personnelId)
        {
            var personnel = await GetById(personnelId);
            if (personnel == null)
                return "Invalid id provided";

            return await Remove(personnel);
        }

        public async Task<string> Remove(Personnel personnel)
        {
            _unitOfWork.Personnel.Delete(personnel);
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
                    .Include(q => q.TEMSUser)
                    .Include(q => q.Tickets)
                    .Include(q => q.EquipmentAllocations)
                    .Include(q => q.KeyAllocations)))
                .FirstOrDefault();

            return personnel;
        }
    }
}
