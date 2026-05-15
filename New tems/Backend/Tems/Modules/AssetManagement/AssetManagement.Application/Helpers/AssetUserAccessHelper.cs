using System.Security.Claims;
using UserManagement.Infrastructure.Repositories;

namespace AssetManagement.Application.Helpers;

internal static class AssetUserAccessHelper
{
    public static async Task<HashSet<string>> ResolveCurrentUserIdentifiersAsync(
        ClaimsPrincipal? principal,
        IUserRepository userRepository,
        CancellationToken cancellationToken = default)
    {
        var identifiers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        void AddIfPresent(string? value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                identifiers.Add(value.Trim());
            }
        }

        var subject = principal?.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? principal?.FindFirstValue("sub")
            ?? principal?.FindFirstValue("preferred_username")
            ?? principal?.FindFirstValue("upn")
            ?? principal?.FindFirstValue("email");

        AddIfPresent(subject);
        AddIfPresent(principal?.FindFirstValue("preferred_username"));
        AddIfPresent(principal?.FindFirstValue(ClaimTypes.Email));
        AddIfPresent(principal?.FindFirstValue("email"));

        if (!string.IsNullOrWhiteSpace(subject))
        {
            var user = await userRepository.GetByIdAsync(subject, cancellationToken)
                ?? await userRepository.GetByIdentityProviderIdAsync(subject, cancellationToken)
                ?? await userRepository.GetByKeycloakIdAsync(subject, cancellationToken)
                ?? await userRepository.GetByEmailAsync(subject, cancellationToken);

            if (user != null)
            {
                AddIfPresent(user.Id);
                AddIfPresent(user.IdentityProviderId);
                AddIfPresent(user.KeycloakId);
                AddIfPresent(user.Email);
                AddIfPresent(user.Name);
            }
        }

        return identifiers;
    }

    public static bool MatchesCurrentUser(string? userId, ISet<string> identifiers)
    {
        return !string.IsNullOrWhiteSpace(userId) && identifiers.Contains(userId.Trim());
    }
}
