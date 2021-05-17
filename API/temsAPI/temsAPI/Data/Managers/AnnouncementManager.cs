﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.ViewModels.Announcement;

namespace temsAPI.Data.Managers
{
    public class AnnouncementManager : EntityManager
    {
        public AnnouncementManager(IUnitOfWork unitOfWork, ClaimsPrincipal user) : base(unitOfWork, user)
        {
        }

        public async Task<string> Create(AddAnnouncementViewModel viewModel)
        {
            string validationStatus = viewModel.Validate();
            if (validationStatus != null)
                return validationStatus;

            Announcement announcement = new Announcement
            {
                Id = Guid.NewGuid().ToString(),
                DateCreated = DateTime.Now,
                Message = viewModel.Text,
                Title = viewModel.Title
            };

            await _unitOfWork.Announcements.Create(announcement);
            await _unitOfWork.Save();

            return null;
        }

        public async Task<List<ViewAnnouncementViewModel>> GetAnnouncements(int skip = 0, int take = int.MaxValue)
        {
            var announcements = (await _unitOfWork.Announcements
                    .FindAll<ViewAnnouncementViewModel>(
                        where: q => !q.IsArchieved,
                        select: q => ViewAnnouncementViewModel.FromModel(q)
                    )).OrderBy(q => q.DateCreated).ToList();

            return announcements;
        }
    }
}