using SIC_Parser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using temsAPI.Contracts;

namespace temsAPI.Services.SICServices
{
    public class SICValidator
    {
        private IUnitOfWork _unitOfWork;

        public SICValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Validates a computer instance
        /// </summary>
        /// <param name="computer"></param>
        /// <param name="unitOfWork"></param>
        /// <returns>Null if everything is ok and the computer can be registered safely. Otherwise, returns an error message</returns>
        public async Task<string> ValidateAgainstUnitOfWork(Computer computer)
        {
            // # Validates the computer model itself
            string modelValidationResult = computer.Validate();
            if(modelValidationResult != null)
                return modelValidationResult;

            // # Check for TEMSID or SerialNumber redundancy
            if (await TEMSIDExists(computer.TEMSID))
                return $"An equipment with the [{computer.TEMSID}] TEMSID already exists.";

            if (await SerialNumberExists(computer.Motherboards[0].SerialNumber))
                return $"An equipment with the [{computer.Motherboards[0].SerialNumber}] SerialNumber already exists.";

            // # Check for Serial number redundancy among equipment's children
            var childrenSerialNumbers =
                computer.CPUs.Select(q => q.TEMSSerialNumber)
                .Concat(computer.GPUs.Select(q => q.TEMSSerialNumber))
                .Concat(computer.Monitors.Select(q => q.TEMSSerialNumber))
                .Concat(computer.Motherboards.Select(q => q.TEMSSerialNumber))
                .Concat(computer.NetworkInterfaces.Select(q => q.TEMSSerialNumber))
                .Concat(computer.PSUs.Select(q => q.TEMSSerialNumber))
                .Concat(computer.RAMs.Select(q => q.TEMSSerialNumber))
                .Concat(computer.Storages.Select(q => q.TEMSSerialNumber));

            StringBuilder errorStringBuilder = new StringBuilder();
            foreach(var serialNumber in childrenSerialNumbers)
                if(await SerialNumberExists(serialNumber))
                    errorStringBuilder.Append(serialNumber.ToString() + "already exists \n");

            var result = errorStringBuilder.ToString();
            if (!String.IsNullOrEmpty(result))
                return result;

            return null;
        }

        private async Task<bool> TEMSIDExists(string temsID)
        {
            return await _unitOfWork.Equipments
                .isExists(q => !String.IsNullOrEmpty(q.TEMSID) && q.TEMSID == temsID && !q.IsArchieved);
        }

        private async Task<bool> SerialNumberExists(string serialNumber)
        {
            return await _unitOfWork.Equipments
                .isExists(q => !String.IsNullOrEmpty(q.SerialNumber) && q.SerialNumber == serialNumber && !q.IsArchieved);
        }
    }
}
