using AssetManagement.Contract.Commands;
using FastEndpoints;
using MediatR;
using System.Security.Claims;
using UserManagement.Application.Queries;

namespace UserManagement.API.Endpoints.Users;

public class GetMyAssetCountEndpoint(IMediator mediator) : EndpointWithoutRequest<GetUserAssetCountResponse>
{
    public override void Configure()
    {
        Get("/profile/assets/count");
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

        var response = await mediator.Send(new GetAssetCountsByUsersCommand([profile.Id]), ct);
        var totalCount = 0;

        if (response.Success && response.Data.TryGetValue(profile.Id, out var counts))
        {
            totalCount = counts.Values.Sum();
        }

        await Send.OkAsync(new GetUserAssetCountResponse(totalCount), ct);
    }
}
