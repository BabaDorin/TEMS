﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.KeyEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Helpers;
using temsAPI.System_Files;
using temsAPI.ViewModels;
using temsAPI.ViewModels.Key;

namespace temsAPI.Controllers.KeyControllers
{
    public class KeyController : TEMSController
    {
        public KeyController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<TEMSUser> userManager) : base(mapper, unitOfWork, userManager)
        {
        }

        [HttpGet]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> Get()
        {
            try
            {
                var keys = (await _unitOfWork.Keys
                    .FindAll<Key>(
                        where: q => !q.IsArchieved,
                        include: q => q
                        .Include(q => q.Room)
                        .Include(q => q.KeyAllocations).ThenInclude(q => q.Personnel)));

                var keysSimplified = keys.Select(q =>
                {
                    var lastActiveAllocation = q.KeyAllocations.FirstOrDefault(q => q.DateReturned == null);

                    return new ViewKeySimplifiedViewModel()
                    {
                        Id = q.Id,
                        Identifier = q.Identifier,
                        Description = q.Description,
                        Room =
                                (q.Room != null)
                                ? new Option
                                {
                                    Value = q.RoomId,
                                    Label = q.Room.Identifier
                                }
                                : new Option
                                {
                                    Value = "--",
                                    Label = "--"
                                },
                        AllocatedTo =
                                (lastActiveAllocation != null)
                                ? new Option
                                {
                                    Value = lastActiveAllocation.PersonnelID,
                                    Label = lastActiveAllocation.Personnel.Name,
                                }
                                : new Option
                                {
                                    Value = "--",
                                    Label = "--"
                                },
                        TimePassed = (lastActiveAllocation != null)
                                ? $"{(DateTime.Now - lastActiveAllocation.DateAllocated).TotalMinutes:f0} minutes ago"
                                : "--"
                    };
                });

                var viewModel = keysSimplified.ToList();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching keys", ResponseStatus.Fail);
            }
        }

        [HttpGet]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> GetAutocompleteOptions()
        {
            try
            {
                List<Option> viewModel = (await _unitOfWork.Keys
                    .FindAll<Option>(
                        where: q => !q.IsArchieved,
                        include: q => q.Include(q => q.Room),
                        select: q => new Option
                        {
                            Value = q.Id,
                            Label = q.Identifier,
                            Additional = (q.RoomId != null) ? q.RoomId : "--"
                        })).ToList();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching keys autocomplete options", ResponseStatus.Fail);
            }
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Create([FromBody] AddKeyViewModel viewModel)
        {
            try
            {
                // No identifier
                if (String.IsNullOrEmpty(viewModel.Identifier = viewModel.Identifier.Trim()))
                    return ReturnResponse("Please, provide a valid key identifier", ResponseStatus.Fail);

                // Invalid Room
                if (viewModel.RoomId != null)
                    if (!await _unitOfWork.Rooms.isExists(q => q.Id == viewModel.RoomId))
                        ReturnResponse("Invalid room provided", ResponseStatus.Fail);

                if (viewModel.NumberOfCopies > 5)
                    return ReturnResponse("Maximum 5 copies at a time.", ResponseStatus.Fail);

                int keysFailed = 0;
                for (int i = 0; i < viewModel.NumberOfCopies; i++)
                {
                    Key model = _mapper.Map<Key>(viewModel);
                    model.RoomId = viewModel.RoomId;
                    model.Id = Guid.NewGuid().ToString();

                    await _unitOfWork.Keys.Create(model);
                    await _unitOfWork.Save();

                    if (!await _unitOfWork.Keys.isExists(q => q.Id == model.Id))
                        keysFailed++;
                }

                if (keysFailed == 0)
                    return ReturnResponse("Success", ResponseStatus.Success);
                else
                    return ReturnResponse($"{keysFailed} keys have not been created.", ResponseStatus.Fail);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when creating the key record.", ResponseStatus.Fail);
            }
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_ALLOCATE_KEYS)]
        public async Task<JsonResult> CreateAllocation([FromBody] AddKeyAllocation viewModel)
        {
            try
            {
                // No keys or personnel provided
                if (viewModel.KeyIds.Count == 0 || String.IsNullOrEmpty(viewModel.PersonnelId))
                    return ReturnResponse("At least one key and one personnel is needed in order to create an allocation", ResponseStatus.Fail);

                // PersonnelId validity
                if (!await _unitOfWork.Personnel.isExists(q => q.Id == viewModel.PersonnelId))
                    return ReturnResponse("Invalid personnel provided", ResponseStatus.Fail);

                // For each key, last allocation is closed and a new one is created
                List<string> keysWhereFailed = new List<string>();
                foreach (string keyId in viewModel.KeyIds)
                {
                    Key key = (await _unitOfWork.Keys
                        .Find<Key>(
                            where: q => q.Id == keyId,
                            include: q => q.Include(q => q.KeyAllocations))
                        ).FirstOrDefault();

                    if (key == null)
                    {
                        keysWhereFailed.Add(keyId);
                        continue;
                    }

                    // Close previous opened allocation
                    key.KeyAllocations
                        .Where(q => q.DateReturned == null)
                        .ToList()
                        .ForEach(q => q.DateReturned = DateTime.Now);

                    // New allocation is created
                    KeyAllocation allocation = new KeyAllocation
                    {
                        Id = Guid.NewGuid().ToString(),
                        KeyID = keyId,
                        PersonnelID = viewModel.PersonnelId,
                        DateAllocated = DateTime.Now,
                    };

                    await _unitOfWork.KeyAllocations.Create(allocation);
                    await _unitOfWork.Save();

                    if (!await _unitOfWork.KeyAllocations.isExists(q => q.Id == allocation.Id))
                        keysWhereFailed.Add(keyId);
                }

                if (keysWhereFailed.Count > 0)
                    return ReturnResponse($"{keysWhereFailed.Count} keys could not be allocated", ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when creating the allocation", ResponseStatus.Fail);
            }
        }

        [HttpGet("key/getallocations/{keyId}/{roomId}/{personnelId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> GetAllocations(string keyId, string roomId, string personnelId)
        {
            try
            {
                // Invalid keyId provided
                if (keyId != "any" && !await _unitOfWork.Keys.isExists(q => q.Id == keyId))
                    return ReturnResponse("Invalid key provided", ResponseStatus.Fail);

                // Invalid roomId provided
                if(roomId != "any" && !await _unitOfWork.Rooms.isExists(q => q.Id == roomId))
                    return ReturnResponse("Invalid room provided", ResponseStatus.Fail);

                // Invalid personnelId provided
                if (personnelId != "any" && !await _unitOfWork.Personnel.isExists(q => q.Id == personnelId))
                    return ReturnResponse("Invalid personnel provied", ResponseStatus.Fail);

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

                List<ViewKeyAllocationViewModel> viewModel = (await _unitOfWork.KeyAllocations
                    .FindAll<ViewKeyAllocationViewModel>(
                        where: finalExpression,
                        include: q => q.Include(q => q.Personnel)
                                       .Include(q => q.Key).ThenInclude(q => q.Room),
                        select: q => new ViewKeyAllocationViewModel
                        {
                            Id = q.Id,
                            DateAllocated = q.DateAllocated,
                            DateReturned = q.DateReturned,
                            Personnel = new Option
                            {
                                Value = q.PersonnelID,
                                Label = q.Personnel.Name,
                                Additional = string.Join(", ", q.Personnel.Positions.Select(q => q.Name))
                            },
                            Key = new Option
                            {
                                Value = q.KeyID,
                                Label = q.Key.Identifier,
                            },
                            Room = new Option
                            {
                                Value = (q.Key.RoomId != null)
                                    ? q.Key.RoomId
                                    : "--",
                                Label = (q.Key.RoomId != null)
                                    ? q.Key.Room.Identifier
                                    : "--",
                            }
                        }
                        )).OrderByDescending(q => q.DateAllocated).ToList();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching key allocations", ResponseStatus.Fail);
            }
        }

        [HttpGet("key/markasreturned/{keyId}")]
        [ClaimRequirement(TEMSClaims.CAN_ALLOCATE_KEYS)]
        public async Task<JsonResult> MarkAsReturned(string keyId)
        {
            try
            {
                if (keyId == null || !await _unitOfWork.Keys.isExists(q => q.Id == keyId))
                    return ReturnResponse("No id has been provided", ResponseStatus.Fail);

                var key = (await _unitOfWork.Keys
                    .Find<Key>(
                        where: q => q.Id == keyId,
                        include: q => q.Include(q => q.KeyAllocations.Where(q => q.DateReturned == null))))
                        .FirstOrDefault();

                key.KeyAllocations.ToList().ForEach(q => q.DateReturned = DateTime.Now);
                await _unitOfWork.Save();

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while marking the key as returned", ResponseStatus.Fail);
            }
        }


        [HttpGet("key/archieve/{keyId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Archieve(string keyId)
        {
            try
            {
                var key = (await _unitOfWork.Keys
                    .Find<Key>(q => q.Id == keyId))
                    .FirstOrDefault();

                if (key == null)
                    return ReturnResponse("Invalid id provided", ResponseStatus.Fail);

                string archivationResult = await new ArchieveHelper(_userManager, _unitOfWork)
                    .SetKeyArchivationStatus(keyId, true);
                if (archivationResult != null)
                    return ReturnResponse(archivationResult, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while archieving the key and it's related data", ResponseStatus.Fail);
            }
        }
    }
}
