using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Helpers;

namespace temsAPI.Services.JWT
{
    public class TokenValidatorService
    {
        List<TemsJWT> _blacklistedTokens = new List<TemsJWT>();
        IServiceScopeFactory _serviceScope;

        public TokenValidatorService(IServiceScopeFactory serviceScope)
        {
            _serviceScope = serviceScope;
            Init().Wait();
        }

        private async Task Init()
        {
            await FetchBlacklistedTokens();
        }

        private async Task FetchBlacklistedTokens()
        {
            using (var scope = _serviceScope.CreateScope())
            {
                IUnitOfWork _unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
                _blacklistedTokens = (await _unitOfWork.JWTBlacklist.FindAll<TemsJWT>()).ToList();
            }
        }

        public async Task BlacklistToken(string token)
        {
            using (var scope = _serviceScope.CreateScope())
            {
                IUnitOfWork _unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();

                var tokenModel = new TemsJWT
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = token,
                    ExpirationDate = (new JWTHelper()).GetExpiryTimestamp(token)
                };

                await _unitOfWork.JWTBlacklist.Create(tokenModel);
                await _unitOfWork.Save();
                _blacklistedTokens.Add(tokenModel);
            }
        }

        public async Task RemoveInvalidTokens()
        {
            using (var scope = _serviceScope.CreateScope())
            {
                IUnitOfWork _unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();

                var toRemove = (await _unitOfWork.JWTBlacklist
                    .FindAll<TemsJWT>(q => q.ExpirationDate > DateTime.UtcNow))
                    .ToList();

                foreach (var token in toRemove)
                    _unitOfWork.JWTBlacklist.Delete(token);
                await _unitOfWork.Save();
            }
        }

        /// <summary>
        /// Checks a token for validity.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>True if token is valid</returns>
        public bool IsValid(string token)
        {
            return !_blacklistedTokens.Any(q => q.Content == token);
        }
    }
}
