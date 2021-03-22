using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Repository;
using IAuthorizationFilter = Microsoft.AspNetCore.Mvc.Filters.IAuthorizationFilter;

namespace temsAPI.System_Files
{
    public class ClaimRequirementFilter : IAuthorizationFilter
    {
        readonly Claim _claim;
        UserManager<TEMSUser> _userManager;
        IUnitOfWork _unitOfWork;

        public ClaimRequirementFilter(Claim claim, UserManager<TEMSUser> userManager, IUnitOfWork unitOfWork)
        {
            _claim = claim;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            //// Validating token (If it has not been blacklisted)
            string token = context.HttpContext.Request.Headers[HeaderNames.Authorization].ToString().Split(' ')[1];
            if (_unitOfWork.JWTBlacklist.isExists(q => q.Content == token).Result)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var hasClaim = context.HttpContext.User.Claims.Any(c => c.Type == _claim.Type);
            if (!hasClaim)
                context.Result = new ForbidResult();
        }
    }

    public class ClaimRequirementAttribute : TypeFilterAttribute
    {
        public ClaimRequirementAttribute(string claimType) : base(typeof(ClaimRequirementFilter))
        {
            Arguments = new object[] { new Claim(claimType, "ye") };
        }
    }
}
