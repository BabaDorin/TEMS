using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using SIC_Parser;
using SIC_Parser.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Asn1;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Controllers;
using temsAPI.ViewModels.Equipment;

namespace temsAPI.Services.SICServices
{
    public class SICService
    {
        IUnitOfWork _unitOfWork;

        public SICService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
            var sicParser = new SICParser();
            Stopwatch sw = new Stopwatch();
            foreach (var file in sicFiles)
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
                    var parseResult = sicParser.ParseSICStream(fileContent);
                    sw.Stop();
                    bulkUploadResult.Add(new SICFileUploadResultViewModel
                    {
                        FileName = file.FileName,
                        Status = (parseResult == null) ? ResponseStatus.Success : ResponseStatus.Fail,
                        EllapsedMiliseconds = (int)sw.ElapsedMilliseconds,
                        Message = (parseResult == null) ? "Succes!" : parseResult
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
