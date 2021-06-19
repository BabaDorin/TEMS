using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.CommunicationEntities;

namespace temsAPI.ViewModels.Log
{
    public class ViewLogViewModel
    {
        public string Id { get; set; }
        public DateTime DateCreated { get; set; }
        public IOption CreatedBy { get; set; }
        public string Text { get; set; }
        public IOption Equipment { get; set; }
        public IOption Room { get; set; }
        public IOption Personnel { get; set; }
        public IOption LogType { get; set; }
        public bool IsImportant { get; set; }

        public static ViewLogViewModel FromModel(Data.Entities.CommunicationEntities.Log log)
        {
            return new ViewLogViewModel
            {
                Id = log.Id,
                DateCreated = log.DateCreated,
                CreatedBy = log.CreatedBy == null
                ? null
                : new Option
                {
                    Value = log.CreatedByID,
                    Label = log.CreatedBy.FullName ?? log.CreatedBy.UserName
                },
                Text = log.Text,
                LogType = (log.LogType == null)
                ? null
                : new Option
                {
                    Value = log.LogType.Id,
                    Label = log.LogType.Type
                },
                Equipment = (log.Equipment == null)
                ? null
                : new Option
                {
                    Value = log.Equipment.Id,
                    Label = log.Equipment.TemsIdOrSerialNumber
                },
                Room = (log.Room == null)
                ? null
                : new Option
                {
                    Value = log.Room.Id,
                    Label = log.Room.Identifier
                },
                Personnel = (log.Personnel == null)
                ? null
                : new Option
                {
                    Value = log.Personnel.Id,
                    Label = log.Personnel.Name
                },
                IsImportant = log.IsImportant
            };
        }
    }
}
