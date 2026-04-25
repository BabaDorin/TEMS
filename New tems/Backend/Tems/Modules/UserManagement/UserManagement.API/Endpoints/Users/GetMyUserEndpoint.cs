using FastEndpoints;
using MediatR;
using System.Security.Claims;
using UserManagement.Application.Commands;
using UserManagement.Application.Queries;
using UserManagement.Contract.Commands;
using UserManagement.Contract.DTOs;

namespace UserManagement.API.Endpoints.Users;

public class GetMyUserEndpoint(IMediator mediator) : EndpointWithoutRequest<UserDto>
{
    public override void Configure()
    {
        Get("/users/me");
        Policies("Authenticated");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var identityProviderId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;

        var name = User.FindFirst("name")?.Value
            ?? $"{User.FindFirst("given_name")?.Value} {User.FindFirst("family_name")?.Value}".Trim();

        var email = User.FindFirst(ClaimTypes.Email)?.Value
            ?? User.FindFirst("email")?.Value;

        var avatarUrl = User.FindFirst("picture")?.Value
            ?? User.FindFirst("avatar")?.Value;

        if (string.IsNullOrWhiteSpace(identityProviderId))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            name = User.FindFirst("preferred_username")?.Value ?? "Unknown User";
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            email = "no-email@unknown.local";
        }

        var profile = await mediator.Send(new GetOrCreateProfileQuery(
            identityProviderId,
            name,
            email,
            avatarUrl
        ), ct);

        var result = await mediator.Send(new GetUserByIdCommand(profile.Id), ct);

        if (result == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(result, ct);
    }
}
