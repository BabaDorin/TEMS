using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using System.Linq;
using System.Security.Claims;
using temsAPI.Services.JWT;
using IAuthorizationFilter = Microsoft.AspNetCore.Mvc.Filters.IAuthorizationFilter;

namespace temsAPI.System_Files
{
    public class ClaimRequirementFilter : IAuthorizationFilter
    {
        readonly Claim[] _claims;
        TokenValidatorService _tokenValidatorService;

        public ClaimRequirementFilter(
            string[] claims, 
            TokenValidatorService tokenValidatorService)
        {
            _tokenValidatorService = tokenValidatorService;

            // The "Can manage system configuration" claim grants access to everything
            _claims = claims
                .Select(q => new Claim(q, "ye"))
                .Append(new Claim(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION, "ye"))
                .ToArray();
        }
            
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Case 1: Not authenticated
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Case 2: Blacklisted token
            string token = context.HttpContext.Request.Headers[HeaderNames.Authorization].ToString().Split(' ')[1];
            if (!_tokenValidatorService.IsValid(token))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            
            // Case 3: No match between required and owned claims
            var hasClaim = context.HttpContext.User.Claims.Any(c => _claims.Any(c1 => c1.Type == c.Type));
            if (!hasClaim)
                context.Result = new ForbidResult();
        }
    }

    public class ClaimRequirementAttribute : TypeFilterAttribute
    {
        public ClaimRequirementAttribute(params string[] claimTypes) : base(typeof(ClaimRequirementFilter))
        {
            Arguments = new object[] { claimTypes };
        }
    }
}
