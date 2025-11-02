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
        public string Additional { get; set; }

        public Option()
        {

        }

        public Option(string value, string label, string additional = null)
        {
            Value = value;
            Label = label;
            Additional = additional;
        }
    }
}
