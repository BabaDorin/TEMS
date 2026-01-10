using MediatR;
using UserManagement.Contract.Responses;

namespace UserManagement.Application.Queries;

public record GetOrCreateProfileQuery(
    string IdentityProviderId,
    string Name,
    string Email,
    string? AvatarUrl
) : IRequest<GetProfileResponse>;
