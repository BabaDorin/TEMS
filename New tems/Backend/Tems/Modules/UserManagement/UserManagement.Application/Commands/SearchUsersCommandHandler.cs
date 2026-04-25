using MediatR;
using Tems.Common.Tenant;
using UserManagement.Contract.Commands;
using UserManagement.Contract.DTOs;
using UserManagement.Infrastructure.Repositories;

namespace UserManagement.Application.Commands;

public class SearchUsersCommandHandler(
    IUserRepository userRepository,
    ITenantContext tenantContext
) : IRequestHandler<SearchUsersCommand, List<UserLookupDto>>
{
    public async Task<List<UserLookupDto>> Handle(SearchUsersCommand request, CancellationToken cancellationToken)
    {
        var users = await userRepository.SearchByNameAsync(
            request.Name,
            request.Take,
            tenantContext.TenantId,
            cancellationToken
        );

        return users.Select(user =>
        {
            var displayName = !string.IsNullOrWhiteSpace(user.Name)
                ? user.Name
                : !string.IsNullOrWhiteSpace(user.Email)
                    ? user.Email
                    : user.Id;

            if (!string.IsNullOrWhiteSpace(user.Email) && !string.Equals(displayName, user.Email, StringComparison.OrdinalIgnoreCase))
            {
                displayName = $"{displayName} ({user.Email})";
            }

            return new UserLookupDto(
                Id: user.Id,
                Name: user.Name,
                Email: user.Email,
                DisplayName: displayName
            );
        }).ToList();
    }
}
