using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.LibraryEntities;
using temsAPI.Helpers.StaticFileHelpers;
using temsAPI.ViewModels.Library;

namespace temsAPI.Data.Managers
{
    public class LibraryManager : EntityManager
    {
        private LibraryItemFileHandler _fileHandler;
        public LibraryManager(IUnitOfWork unitOfWork, ClaimsPrincipal user) : base(unitOfWork, user)
        {
            _fileHandler = new LibraryItemFileHandler();
        }

        public async Task<string> UploadFile(IFormFile file, string name, string description)
        {
            if (file == null)
                return "Null file??";

            AddLibraryItemViewModel viewModel = new AddLibraryItemViewModel
            {
                DisplayName = name,
                Description = description,
                ActualName = _fileHandler.GetSanitarizedUniqueActualName(file),
            };

            string dbPath = _fileHandler.CompressAndSave(file, viewModel.ActualName);

            if (dbPath == null)
                return "File could not be uploaded";

            LibraryItem model = new LibraryItem
            {
                Id = Guid.NewGuid().ToString(),
                ActualName = viewModel.ActualName,
                DateUploaded = DateTime.Now,
                DbPath = dbPath,
                Description = viewModel.Description,
                DisplayName = viewModel.DisplayName,
                FileSize = file.Length,
            };

            await _unitOfWork.LibraryItems.Create(model);
            await _unitOfWork.Save();

            return null;
        }

        public async Task<List<ViewLibraryItemViewModel>> GetItems(int skip = 0, int take = int.MaxValue)
        {
            var items = (await _unitOfWork
                .LibraryItems
                .FindAll<ViewLibraryItemViewModel>(
                    orderBy: q => q.OrderByDescending(q => q.Downloads),
                    select: q => ViewLibraryItemViewModel.FromModel(q)
                )).ToList();

            return items;
        }

        public async Task<string> Remove(string itemId)
        {
            // Invalid id provided
            if (!await _unitOfWork.LibraryItems.isExists(q => q.Id == itemId))
                return "Invalid item provided";

            LibraryItem libraryItem = await GetById(itemId);

            _fileHandler.DeleteFile(libraryItem.DbPath);
            _unitOfWork.LibraryItems.Delete(libraryItem);
            await _unitOfWork.Save();

            return null;
        }

        public async Task<LibraryItem> GetById(string itemId)
        {
            var item = (await _unitOfWork.LibraryItems.Find<LibraryItem>(q => q.Id == itemId))
                .FirstOrDefault();

            return item;
        }

        public async Task<MemoryStream> GetLibraryItemMemoryStream(LibraryItem item)
        {
            string filePath = GetFilePath(item);

            if (!System.IO.File.Exists(filePath))
            {
                await Remove(item.Id);
                throw new Exception("The selected library item does not point to an existing file." +
                    " The library item has been removed because it's not associated with any real file.");
            }

            var memory = new MemoryStream();
            await using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return memory;
        }

        public string GetFilePath(LibraryItem item)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), item.DbPath);
        }

        public async Task UpdateDownloadsCounter(LibraryItem item)
        {
            ++item.Downloads;
            _unitOfWork.LibraryItems.Update(item);
            await _unitOfWork.Save();
        } 
    }
}
