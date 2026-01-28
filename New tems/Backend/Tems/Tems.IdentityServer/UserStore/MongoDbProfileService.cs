using System.Security.Claims;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using MongoDB.Driver;
using Tems.Common.Identity;

namespace Tems.IdentityServer.UserStore;

public class MongoDbProfileService : IProfileService
{
    private readonly IMongoCollection<User> _users;

    public MongoDbProfileService(IMongoDatabase database)
    {
        _users = database.GetCollection<User>("users");
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var userId = context.Subject.GetSubjectId();
        var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();

        if (user == null) return;

        // Duende only provides authentication claims
        // Roles and permissions are managed by Keycloak
        var claims = new List<Claim>
        {
            new Claim("sub", user.Id),
            new Claim("username", user.Username),
            new Claim("email", user.Email),
            new Claim("name", user.FullName),
        };

        if (!string.IsNullOrEmpty(user.PhoneNumber))
        {
            claims.Add(new Claim("phone_number", user.PhoneNumber));
        }

        context.IssuedClaims = claims;
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var userId = context.Subject.GetSubjectId();
        var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
        context.IsActive = user?.IsActive ?? false;
    }
}
