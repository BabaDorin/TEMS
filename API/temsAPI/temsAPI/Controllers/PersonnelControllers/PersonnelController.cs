using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.ViewModels;
using temsAPI.ViewModels.Personnel;

namespace temsAPI.Controllers.PersonnelControllers
{
    public class PersonnelController : TEMSController
    {
        public PersonnelController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<TEMSUser> userManager) : base(mapper, unitOfWork, userManager)
        {
        }

        [HttpGet]
        public async Task<JsonResult> GetPositions()
        {
            try
            {
                List<Option> viewModel = (await _unitOfWork.PersonnelPositions.FindAll<Option>(
                    where: q => !q.IsArchieved,
                    select: q => new Option
                    {
                        Value = q.Id,
                        Label = q.Name
                    }
                    )).ToList();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching positions", ResponseStatus.Fail);
            }
        }

        [HttpPost]
        public async Task<JsonResult> Create([FromBody] AddPersonnelViewModel viewModel)
        {
            try
            {
                // Invalid Name
                if (String.IsNullOrEmpty(viewModel.Name = viewModel.Name.Trim()))
                    return ReturnResponse("Invalid personnel name provided", ResponseStatus.Fail);

                // Checking for invalid personnel positions
                foreach (Option position in viewModel.Positions)
                    if (!await _unitOfWork.PersonnelPositions
                        .isExists(q => q.Id == position.Value && !q.IsArchieved))
                        return ReturnResponse("One or more positions are invalid.", ResponseStatus.Fail);

                // It might be valid enough
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

                if (!await _unitOfWork.Personnel.isExists(q => q.Id == model.Id))
                    return ReturnResponse("Fail", ResponseStatus.Fail);
                else
                    return ReturnResponse("Success!", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when creating the personnel", ResponseStatus.Fail);
            }
        }

        [HttpGet("/personnel/getsimplified/{pageNumber}/{recordsPerPage}")]
        public async Task<JsonResult> GetSimplified(int pageNumber, int recordsPerPage)
        {
            try
            {
                // Invalid page number or records per page provided
                if (pageNumber < 0 || recordsPerPage < 0 || pageNumber > 1000 || recordsPerPage > 1000)
                    return ReturnResponse("Invalid parameters, please provide real number representing page number" +
                        "and how many records are displayed per page", ResponseStatus.Fail);

                List<ViewPersonnelSimplifiedViewModel> viewModel = (await _unitOfWork.Personnel
                    .FindAll<ViewPersonnelSimplifiedViewModel>(
                        where: q => !q.IsArchieved,
                        include: q => q.Include(q => q.PersonnelEquipmentAllocations.Where(q => q.DateReturned == null))
                                       .Include(q => q.Positions)
                                       .Include(q => q.Tickets.Where(q => q.DateClosed == null)),
                        select: q => new ViewPersonnelSimplifiedViewModel
                        {
                            Id = q.Id,
                            Name = q.Name,
                            ActiveTickets = q.Tickets.Count,
                            AllocatedEquipments = q.PersonnelEquipmentAllocations.Count,
                            Positions = (q.Positions != null)
                                ? string.Join(", ", q.Positions.Select(q => q.Name))
                                : ""
                        })).ToList();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching personnel records", ResponseStatus.Fail);
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetAllAutocompleteOptions()
        {
            try
            {
                List<Option> viewModel = (await _unitOfWork.Personnel
                    .FindAll<Option>(
                        where: q => !q.IsArchieved,
                        include: q => q.Include(q => q.Positions),
                        select: q => new Option
                        {
                            Value = q.Id,
                            Label = q.Name,
                            Additional = string.Join(", ", q.Positions.Select(q => q.Name))
                        })).ToList();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching autocomplete options", ResponseStatus.Fail);
            }
        }
    }
}
