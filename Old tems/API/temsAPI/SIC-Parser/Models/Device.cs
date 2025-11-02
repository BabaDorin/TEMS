using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIC_Parser.Models
{
    public class Device
    {
        /// <summary>
        /// Displays properties along with their values in Output (for debugging).
        /// </summary>
        public void DisplayData()
        {
            Debug.WriteLine(this.GetType().Name + "----------------------------------");

            foreach (var Prop in this.GetType().GetProperties())
                Debug.WriteLine($"{Prop.Name}: {Prop.GetValue(this)}");

            Debug.WriteLine("");
        }
    }
}
