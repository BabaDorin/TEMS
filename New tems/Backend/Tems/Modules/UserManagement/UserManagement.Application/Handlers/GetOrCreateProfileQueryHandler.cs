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
        // Try to get existing user by Identity Provider ID
        var existingUser = await userRepository.GetByIdentityProviderIdAsync(
            request.IdentityProviderId, 
            cancellationToken
        );

        Infrastructure.Entities.User dbUser;

        if (existingUser == null)
        {
            // Create new user
            dbUser = new Infrastructure.Entities.User
            {
                Name = request.Name,
                Email = request.Email,
                AvatarUrl = request.AvatarUrl,
                IdentityProviderId = request.IdentityProviderId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            dbUser = await userRepository.CreateAsync(dbUser, cancellationToken);
        }
        else
        {
            // Check if any data has changed and update if necessary
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
