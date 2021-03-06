using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels
{
    public class Option : IOption
    {
        public string Value { get; set; }
        public string Label { get; set; }
#nullable enable
        public string? Additional { get; set; }
#nullable disable
    }
}
