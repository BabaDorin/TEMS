using System.Security.Claims;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using Tems.Common.Identity;

namespace Tems.IdentityServer.UserStore;

public class MongoDbResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
{
    private readonly IMongoCollection<User> _users;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ILogger<MongoDbResourceOwnerPasswordValidator> _logger;

    public MongoDbResourceOwnerPasswordValidator(
        IMongoDatabase database,
        IPasswordHasher<User> passwordHasher,
        ILogger<MongoDbResourceOwnerPasswordValidator> logger)
    {
        _users = database.GetCollection<User>("users");
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
    {
        try
        {
            var user = await _users.Find(u => u.Username == context.UserName).FirstOrDefaultAsync();

            if (user != null && user.IsActive)
            {
                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, context.Password);

                if (result == PasswordVerificationResult.Success || 
                    result == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    _logger.LogInformation("User {Username} authenticated successfully", context.UserName);
                    
                    context.Result = new GrantValidationResult(
                        subject: user.Id,
                        authenticationMethod: "password",
                        claims: new[]
                        {
                            new Claim("username", user.Username),
                            new Claim("email", user.Email)
                        });
                    return;
                }
            }

            _logger.LogWarning("Authentication failed for user {Username}", context.UserName);
            context.Result = new GrantValidationResult(
                TokenRequestErrors.InvalidGrant,
                "Invalid credentials");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating credentials for user {Username}", context.UserName);
            context.Result = new GrantValidationResult(
                TokenRequestErrors.InvalidGrant,
                "Authentication error");
        }
    }
}
