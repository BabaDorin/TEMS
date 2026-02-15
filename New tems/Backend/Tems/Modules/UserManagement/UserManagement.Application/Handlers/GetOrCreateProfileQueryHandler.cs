using MediatR;
using UserManagement.Application.Domain;
using UserManagement.Application.Queries;
using UserManagement.Contract.Responses;
using UserManagement.Infrastructure.Repositories;

namespace UserManagement.Application.Handlers;

public class GetOrCreateProfileQueryHandler(IUserRepository userRepository) 
    : IRequestHandler<GetOrCreateProfileQuery, GetProfileResponse>
{
    public async Task<GetProfileResponse> Handle(GetOrCreateProfileQuery request, CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.GetByIdentityProviderIdAsync(
            request.IdentityProviderId, 
            cancellationToken
        );

        existingUser ??= await userRepository.GetByKeycloakIdAsync(
            request.IdentityProviderId,
            cancellationToken
        );

        existingUser ??= await userRepository.GetByEmailAsync(
            request.Email,
            cancellationToken
        );

        Infrastructure.Entities.User dbUser;

        if (existingUser == null)
        {
            dbUser = new Infrastructure.Entities.User
            {
                Name = request.Name,
                Email = request.Email,
                AvatarUrl = request.AvatarUrl,
                IdentityProviderId = request.IdentityProviderId,
                KeycloakId = request.IdentityProviderId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            dbUser = await userRepository.CreateAsync(dbUser, cancellationToken);
        }
        else
        {
            bool hasChanges = false;

            if (existingUser.Name != request.Name)
            {
                existingUser.Name = request.Name;
                hasChanges = true;
            }

            if (existingUser.Email != request.Email)
            {
                existingUser.Email = request.Email;
                hasChanges = true;
            }

            if (existingUser.AvatarUrl != request.AvatarUrl)
            {
                existingUser.AvatarUrl = request.AvatarUrl;
                hasChanges = true;
            }

            if (existingUser.IdentityProviderId != request.IdentityProviderId)
            {
                existingUser.IdentityProviderId = request.IdentityProviderId;
                hasChanges = true;
            }

            if (existingUser.KeycloakId != request.IdentityProviderId)
            {
                existingUser.KeycloakId = request.IdentityProviderId;
                hasChanges = true;
            }

            if (hasChanges)
            {
                dbUser = await userRepository.UpdateAsync(existingUser, cancellationToken);
            }
            else
            {
                dbUser = existingUser;
            }
        }

        return new GetProfileResponse(
            dbUser.Id,
            dbUser.Name,
            dbUser.Email,
            dbUser.AvatarUrl,
            dbUser.IdentityProviderId,
            dbUser.CreatedAt,
            dbUser.UpdatedAt
        );
    }
}
