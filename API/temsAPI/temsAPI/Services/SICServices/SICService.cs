using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SIC_Parser.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Controllers;
using temsAPI.System_Files.Exceptions;
using temsAPI.ViewModels.Equipment;

namespace temsAPI.Services.SICServices
{
    public class SICService
    {
        IUnitOfWork _unitOfWork;
        ILogger _logger;

        public SICService(IUnitOfWork unitOfWork, ILogger<SICService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Makes TEMS compatible with Sistem Info Collector by registering SIC Types along with their properties.
        /// Note: TEMS aldready contains properties that SIC needs, then those properties will be marked
        /// as non-editable (Property name, datatype and description).
        /// </summary>
        /// <returns>Returns null if everything is ok, otherwise - the error message.</returns>
        public async Task<string> IntegrateSIC()
        {
            var sicIntegrationService = new SIC_IntegrationService(_unitOfWork);
            return await sicIntegrationService.PrepareDBForSICIntegration();
        }

        /// <summary>
        /// Extracts and validates computers within a IFormFileCollection.
        /// </summary>
        /// <param name="sicFiles"></param>
        /// <returns>A list of results, one per computer (file).</returns>
        public async Task<List<SICFileUploadResultViewModel>> ValidateAndRegisterComputers(IFormFileCollection sicFiles)
        {
            var bulkUploadResult = new List<SICFileUploadResultViewModel>();
            Stopwatch sw = new Stopwatch();
            foreach (var file in sicFiles)
            {
                try
                {
                    if (Path.GetExtension(file.FileName) != ".json")
                    {
                        bulkUploadResult.Add(new SICFileUploadResultViewModel
                        {
                            FileName = file.FileName,
                            Message = "Keep it for yourself ;)",
                            Status = ResponseStatus.Fail
                        });
                        continue;
                    }

                    using (var stream = file.OpenReadStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var fileContent = await reader.ReadToEndAsync();

                        sw.Start();
                        var computer = JsonConvert.DeserializeObject<Computer>(fileContent);
                        string registerResult = await RegisterComputer(computer);
                        sw.Stop();

                        bulkUploadResult.Add(new SICFileUploadResultViewModel
                        {
                            FileName = file.FileName,
                            Status = (registerResult == null) ? ResponseStatus.Success : ResponseStatus.Fail,
                            EllapsedMiliseconds = (int)sw.ElapsedMilliseconds,
                            Message = (registerResult == null) ? "Succes!" : registerResult + " | Make sure SIC has been integrated within your system."
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.Log(LogLevel.Error, ex, "Error SIC Parsing");
                    
                    sw.Stop();
                    bulkUploadResult.Add(new SICFileUploadResultViewModel
                    {
                        FileName = file.FileName,
                        Status = ResponseStatus.Fail,
                        EllapsedMiliseconds = (int)sw.ElapsedMilliseconds,
                        Message = "I could not read the file ;( \nDetails: " + ex.Message
                    });
                }
            }

            return bulkUploadResult;
        }

        /// <summary>
        /// Registers a computer based on data being provided by the model (sicComputer)
        /// </summary>
        /// <param name="sicComputer"></param>
        /// <returns>Returns null if everythink is ok, otherwise - error message.</returns>
        public async Task<string> RegisterComputer(Computer sicComputer)
        {
            SIC_Register sicRegister = new SIC_Register(_unitOfWork);
            return await sicRegister.RegisterComputer(sicComputer);
        }
    }
}
