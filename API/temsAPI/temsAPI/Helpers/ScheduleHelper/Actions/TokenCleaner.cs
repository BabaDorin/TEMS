using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Services.JWT;

namespace temsAPI.Helpers.ScheduleHelper.Actions
{
    public class TokenCleaner : IScheduledAction
    {
        TokenValidatorService _tokenValidatorService;

        public TokenCleaner(TokenValidatorService tokenValidatorService)
        {
            _tokenValidatorService = tokenValidatorService;
        }

        public async Task Start()
        {
            await _tokenValidatorService.RemoveInvalidTokens();
        }
    }
}
