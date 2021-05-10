using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.Ticket
{
    public class ViewTicketSimplifiedViewModel
    {
        public string Id { get; set; }
        public string Problem { get; set; }
        public Option Status { get; set; }
        public Option Label { get; set; }
        public string Description { get; set; }
        public List<Option> Personnel { get; set; }
        public List<Option> Equipments { get; set; }
        public List<Option> Rooms { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateClosed { get; set; }
        public Option ClosedBy { get; set; }
        public List<Option> Assignees { get; set; }
        
        public static ViewTicketSimplifiedViewModel FromModel(Data.Entities.CommunicationEntities.Ticket ticket)
        {
            return new ViewTicketSimplifiedViewModel
            {
                Id = ticket.Id,
                Problem = ticket.Problem,
                DateClosed = ticket.DateClosed,
                DateCreated = ticket.DateCreated,
                Description = ticket.Description,
                Equipments = ticket.Equipments.Select(q => new Option
                {
                    Value = q.Id,
                    Label = q.TemsIdOrSerialNumber
                }).ToList(),
                Label = new Option
                {
                    Value = ticket.Label?.Id,
                    Label = ticket.Label?.Name,
                    Additional = ticket?.Label?.ColorHex
                },
                Personnel = ticket.Personnel.Select(q => new Option
                {
                    Value = q.Id,
                    Label = q.Name
                }).ToList(),
                Rooms = ticket.Rooms.Select(q => new Option
                {
                    Value = q.Id,
                    Label = q.Identifier,
                    Additional = string.Join(", ", q.Labels)
                }).ToList(),
                Assignees = ticket.Assignees.Select(q => new Option
                {
                    Value = q.Id,
                    Label = q.FullName ?? q.UserName,
                }).ToList(),
                Status = new Option
                {
                    Value = ticket.Status.Id,
                    Label = ticket.Status.Name,
                    Additional = ticket.Status.ImportanceIndex.ToString()
                },
                ClosedBy = (ticket.ClosedById == null)
                        ? null
                        : new Option
                        {
                            Label = ticket.ClosedBy.FullName ?? ticket.ClosedBy.UserName,
                            Value = ticket.ClosedById
                        }
            };
        }
    }
}
