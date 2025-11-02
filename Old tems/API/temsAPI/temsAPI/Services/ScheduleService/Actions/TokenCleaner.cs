using System.Threading.Tasks;
using temsAPI.Services.JWT;

namespace temsAPI.Services.Actions
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
