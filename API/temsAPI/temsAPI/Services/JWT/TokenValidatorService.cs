﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Helpers;

namespace temsAPI.Services.JWT
{
    public class TokenValidatorService
    {
        // Blacklisted tokens are those that were invalidated as a result of 
        // signing-out, password change, claims change etc.
        List<TemsJWT> _blacklistedTokens = new();

        // Due to the fact that from backend we can't access user's token (but we want to 
        // make one's token blacklisted) we temporarily store userIds and, on token validation,
        // user's ID from token is checked agains _usersWithBlacklistedTokens to make sure that
        // user's has not been blacklisted by someone.

        // If current userId is blacklisted, it's token is blacklisted 
        // an userId is blacklisted only if the provided token was issued before the blacklisting.
        // Exaple: password changed at 11am 9th of august 2021. All of user's tokens issued before that date
        // will get blacklisted. The ones issued after that date, will be active.

        // UserId remains into the usersWithBlacklistedTokens table. The record will be deleted when the user itself will
        // be deleted.

        // Over time, only the date will update.
        List<UserWithBlacklistedToken> _usersWithBlacklistedTokens = new();
        
        IServiceScopeFactory _serviceScope;
        JwtSecurityTokenHandler _jwtSecurityHandler;

        public TokenValidatorService(IServiceScopeFactory serviceScope)
        {
            _serviceScope = serviceScope;
            _jwtSecurityHandler = new JwtSecurityTokenHandler();
            _ = Task.Factory.StartNew(() => Init());
        }

        private async Task Init()
        {
            await FetchBlacklistedItems();
        }

        /// <summary>
        /// Fetch synchronously blacklisted tokens and user ids (invalid token holders)
        /// </summary>
        /// <returns></returns>
        private async Task FetchBlacklistedItems()
        {
            using (var scope = _serviceScope.CreateScope())
            {
                IUnitOfWork _unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
                _blacklistedTokens = (await _unitOfWork.JWTBlacklist.FindAll<TemsJWT>()).ToList();
                _usersWithBlacklistedTokens = (await _unitOfWork.UserWithBlacklistedTokens.FindAll<UserWithBlacklistedToken>()).ToList();
            }
        }

        /// <summary>
        /// Blacklist a specific token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task BlacklistToken(string token, IUnitOfWork unitOfWork = null)
        {
            var tokenModel = new TemsJWT
            {
                Id = Guid.NewGuid().ToString(),
                Content = token,
                ExpirationDate = (new JWTHelper()).GetExpiryTimestamp(token)
            };

            if(unitOfWork != null)
            {
                await BlacklistToken(tokenModel, unitOfWork);
                return;
            }

            using (var scope = _serviceScope.CreateScope())
            {
                unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
                await BlacklistToken(tokenModel, unitOfWork);
            }
        }

        private async Task BlacklistToken(TemsJWT token, IUnitOfWork unitOfWork)
        {
            await unitOfWork.JWTBlacklist.Create(token);
            await unitOfWork.Save();
            _blacklistedTokens.Add(token);
        }

        /// <summary>
        /// Blacklist the current token of userId when it will be encountered
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task BlacklistUserToken(string userId)
        {
            using (var scope = _serviceScope.CreateScope())
            {
                IUnitOfWork unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();

                // 1. Validation: Valid user ID;
                if (!await unitOfWork.TEMSUsers.isExists(q => q.Id == userId))
                    return;

                // If current userId is already blacklisted, then only the date gets updated
                var record = (await unitOfWork.UserWithBlacklistedTokens
                    .Find<UserWithBlacklistedToken>(q => q.UserID == userId))
                    .FirstOrDefault();

                if (record != null)
                {
                    record.DateBlacklisted = DateTime.UtcNow;
                    await unitOfWork.Save();

                    _usersWithBlacklistedTokens.Find(q => q.UserID == userId).DateBlacklisted = DateTime.UtcNow;
                    return;
                }

                UserWithBlacklistedToken model = new();
                model.UserID = userId;

                await unitOfWork.UserWithBlacklistedTokens.Create(model);
                await unitOfWork.Save();
                _usersWithBlacklistedTokens.Add(model);
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
        public async Task<bool> IsValid(string token)
        {
            // token is considered invalid in any of these scenarios:
            // 1. Token is present within blacklistedTokens collection
            // 2. UserID read from token is contained within userWithBlacklistedTokens collection
            // AND the token generation date is earlier than blacklisting date (token.DateCreated < userBlacklist.DateCreated)
            return !(TokenBlacklisted(token) || await TokenHolderBlacklisted(token));
        }

        private async Task WhitelistUser(UserWithBlacklistedToken tokenHolder, IUnitOfWork unitOfWork)
        {
            try
            {
                unitOfWork.UserWithBlacklistedTokens.Delete(tokenHolder);
                await unitOfWork.Save();
                _usersWithBlacklistedTokens.Remove(tokenHolder);

            }
            catch (Exception)
            {
                // BEFREE: Exeption here is thrown because there are multiple threads accesssing the dbContext and 
                // removing user ID from the collection.
                // Deal with this multi-threading issue as soon as possbile. (mutex - semaphore - lock, find what's more 
                // applicable in this use-case.

                // An approach is needed where each token is validated maximum once per second.
                // If there are several tokens to be validated in 1ms (for example), there is no need to validate each of them
                // Validate the first request, then memoize it. If second request is within 1 second from the first one,
                // then just take the value computed by the first request.

                // The main problem is: How to define which request is the first?
                // There is a very short delay between requests (threads), and this should be taken into consideration.
            }
        }

        private bool TokenBlacklisted(string token)
        {
            return _blacklistedTokens.Any(q => q.Content == token);
        }

        private async Task<bool> TokenHolderBlacklisted(string token)
        {
            var tokenObject = _jwtSecurityHandler.ReadJwtToken(token);
            var userId = tokenObject.Claims.First(q => q.Type == "UserID").Value;
            var blacklistedTokenHolder = _usersWithBlacklistedTokens.FirstOrDefault(q => q.UserID == userId);

            if (blacklistedTokenHolder != null)
            {
                using (var scope = _serviceScope.CreateScope())
                {
                    IUnitOfWork unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();

                    // ghost blacklisting (caused by UserWithBlacklistedTokens table being unsynced with local collection)
                    if (!await unitOfWork.UserWithBlacklistedTokens.isExists(q => q.UserID == blacklistedTokenHolder.UserID))
                    {
                        _usersWithBlacklistedTokens.Remove(blacklistedTokenHolder);
                        return false;
                    }

                    // Case 1: UserId blacklisted & token issued before blacklisting is still active
                    // Action => Blacklist token
                    if (blacklistedTokenHolder.DateBlacklisted > tokenObject.IssuedAt)
                    {
                        await BlacklistToken(token, unitOfWork);
                        //await WhitelistUser(blacklistedTokenHolder, unitOfWork);
                        return true;
                    }

                    // Case 2: User id is indeed blacklisted, but the new token was issued after blacklisting
                    // Action => Do nothing, the token is valid
                    return false;
                }
            }

            return false;
        }
    }
}
