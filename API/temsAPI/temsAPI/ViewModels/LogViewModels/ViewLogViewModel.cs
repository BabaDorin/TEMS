using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.Log
{
    public class ViewLogViewModel
    {
        public string Id { get; set; }
        public DateTime DateCreated { get; set; }
        public string Text { get; set; }
        public IOption Equipment { get; set; }
        public IOption Room { get; set; }
        public IOption Personnel { get; set; }
        public IOption LogType { get; set; }
        public bool IsImportant { get; set; }
    }
}
