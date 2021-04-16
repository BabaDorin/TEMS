using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using SIC_Parser.Models;

namespace SIC_Parser
{
    public class SICParser
    {
        public string ParseSICStream(string stream)
        {
            try
            {
                Computer test = JsonConvert.DeserializeObject<Computer>(stream);


                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
