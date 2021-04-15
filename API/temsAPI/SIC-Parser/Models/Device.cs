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
        /// Generates test data for all of object's properties
        /// </summary>
        public void TestData()
        {
            foreach (var prop in this.GetType().GetProperties())
            {
                // 1) If boolean, set it to true.
                // If it is not boolean, assign a number.
                // If it is a string, it will throw an exception. If so, we catch the exception and assign a string.
                // We can not check a string's type if it's empty. 

                Type T = prop.PropertyType;

                switch (T.ToString())
                {
                    case "System.Int32":
                    case "System.Single":
                    case "System.Double":
                        prop.SetValue(this, 12345);
                        break;
                    case "System.String":
                        prop.SetValue(this, $"Test {prop.Name}");
                        break;
                    case "System.Boolean":
                        prop.SetValue(this, true);
                        break;
                }
            }
        }

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
