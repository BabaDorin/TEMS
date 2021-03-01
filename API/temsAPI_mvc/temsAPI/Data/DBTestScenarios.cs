using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Testing;

namespace temsAPI.Data
{
    public class DBTestScenarios
    {
        public static void TestOnDeleteCascade(ApplicationDbContext dbContext)
        {
            // Add an equipment type
            // Add a property
            // Associate them
            // Delete equipment type
            // See what happens.
            // Write logs

            List<string> logs = new List<string>();

            logs.Add("Part 1: Creating records");

            string etID = Guid.NewGuid().ToString();
            dbContext.EquipmentTypes.Add(new EquipmentType
            {
                ID = etID,
                Type = "Printer"
            });
            logs.Add("EquipmentType table += Printer type");


            string pID = Guid.NewGuid().ToString();
            dbContext.Properties.Add(new Property
            {
                ID = pID,
                DisplayName = "Denumire",
                Name = "Name"
            });
            logs.Add("Property table += Name");

            string petaID = Guid.NewGuid().ToString();
            dbContext.PropertyEquipmentTypeAssociations.Add(new PropertyEquipmentTypeAssociation
            {
                ID = Guid.NewGuid().ToString(),
                PropertyID = pID,
                TypeID = etID
            });
            logs.Add("PropertyEquipmentTypeAssociations table += PrinterType <=> Name");

            dbContext.SaveChanges();
            logs.Add("DBContext Changes have been saved");

            logs.Add("Part 2: Removing records in order to test OnDeleteCascade");

            dbContext.EquipmentTypes.Remove(dbContext.EquipmentTypes.FirstOrDefault(q => q.ID == etID));
            logs.Add("EquipmentType table -= Printer type");
            
            dbContext.SaveChanges();
            logs.Add("DBContext Changes have been saved");

            logs.Add("Expected behavior: Association between Printer Type and Name Property to be removed");

            logs.Add("Success: " + !dbContext.PropertyEquipmentTypeAssociations.Any(q => q.PropertyID == pID));

            dbContext.Properties.Remove(dbContext.Properties.FirstOrDefault(q => q.ID == pID));
            dbContext.SaveChanges();

            logs.Add("DB Set to initial state.");
            logs.Add("Test made on: " + DateTime.Now.ToString());

            WriteTestLogs.WriteTestLog(TestLog.OnDeleteCascade, false, logs);
        }
    }
}
