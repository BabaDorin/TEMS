using AssetManagement.Contract.Commands;
using MediatR;
using Microsoft.Extensions.Logging;
using Tems.Common.Tenant;
using UserManagement.Contract.Commands;
using UserManagement.Contract.DTOs;
using UserManagement.Contract.Responses;
using UserManagement.Infrastructure.Keycloak;
using UserManagement.Infrastructure.Repositories;

namespace UserManagement.Application.Commands;

public class GetAllUsersCommandHandler(
    IUserRepository userRepository,
    IKeycloakClient keycloakClient,
    ITenantContext tenantContext,
    IMediator mediator,
    ILogger<GetAllUsersCommandHandler> logger
) : IRequestHandler<GetAllUsersCommand, GetAllUsersResponse>
{
    public async Task<GetAllUsersResponse> Handle(GetAllUsersCommand request, CancellationToken cancellationToken)
    {
        var tenantId = tenantContext.TenantId;
        
        var (users, totalCount) = await userRepository.GetAllAsync(
            request.PageNumber,
            request.PageSize,
            tenantId,
            cancellationToken
        );

        var userIds = users.Select(u => u.Id).ToList();

        Dictionary<string, Dictionary<string, int>> assetCountsByUser = [];
        try
        {
            var assetCountsResponse = await mediator.Send(
                new GetAssetCountsByUsersCommand(userIds),
                cancellationToken);
            if (assetCountsResponse.Success)
                assetCountsByUser = assetCountsResponse.Data;
        }
        catch
        {
            // Asset counts are non-critical
        }

        // Batch-fetch roles: fetch in parallel for all users with a valid KeycloakId
        var rolesByKeycloakId = new Dictionary<string, List<string>>();
        var usersWithKeycloakId = users
            .Where(u => !string.IsNullOrEmpty(u.KeycloakId))
            .ToList();

        var roleTasks = usersWithKeycloakId.Select(async user =>
        {
            try
            {
                var keycloakRoles = await keycloakClient.GetUserRolesAsync(user.KeycloakId!);
                var roleNames = keycloakRoles
                    .Select(r => r.Name ?? "")
                    .Where(r => !string.IsNullOrEmpty(r))
                    .ToList();
                return (user.KeycloakId!, roleNames);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to fetch roles for user {UserId} (KeycloakId: {KeycloakId})",
                    user.Id, user.KeycloakId);
                return (user.KeycloakId!, new List<string>());
            }
        });

        var roleResults = await Task.WhenAll(roleTasks);
        foreach (var (keycloakId, roles) in roleResults)
        {
            rolesByKeycloakId[keycloakId] = roles;
        }

        var userDtos = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = new List<string>();
            if (!string.IsNullOrEmpty(user.KeycloakId) && rolesByKeycloakId.TryGetValue(user.KeycloakId, out var cachedRoles))
            {
                roles = cachedRoles;
            }

            assetCountsByUser.TryGetValue(user.Id, out var userAssetCounts);

            userDtos.Add(new UserDto(
                Id: user.Id,
                Username: user.Name,
                Email: user.Email,
                FirstName: null,
                LastName: null,
                AvatarUrl: user.AvatarUrl,
                TenantIds: user.TenantIds,
                KeycloakId: user.KeycloakId,
                Roles: roles,
                CreatedAt: user.CreatedAt,
                UpdatedAt: user.UpdatedAt,
                AssetCounts: userAssetCounts
            ));
        }

        return new GetAllUsersResponse(
            Users: userDtos,
            TotalCount: totalCount,
            PageNumber: request.PageNumber,
            PageSize: request.PageSize
        );
    }
}
