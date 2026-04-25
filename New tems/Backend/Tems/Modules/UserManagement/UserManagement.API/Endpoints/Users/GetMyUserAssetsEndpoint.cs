using AssetManagement.Contract.Commands;
using AssetManagement.Contract.DTOs;
using AssetManagement.Contract.Responses;
using FastEndpoints;
using MediatR;
using System.Security.Claims;
using UserManagement.Application.Queries;

namespace UserManagement.API.Endpoints.Users;

public class GetMyUserAssetsEndpoint(IMediator mediator) : EndpointWithoutRequest<GetAllAssetResponse>
{
    public override void Configure()
    {
        Get("/users/me/assets");
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

        var assetTag = Query<string>("assetTag", false);
        var pageNumber = Query<int>("pageNumber", false);
        var pageSize = Query<int>("pageSize", false);

        var filter = new AssetFilterDto(
            AssetTag: string.IsNullOrWhiteSpace(assetTag) ? null : assetTag,
            AssetTypeIds: null,
            DefinitionIds: null,
            DefinitionNames: null,
            AssignedToUserId: profile.Id,
            IncludeArchived: false
        );

        var command = new GetAllAssetCommand(
            Filter: filter,
            PageNumber: pageNumber > 0 ? pageNumber : 1,
            PageSize: pageSize > 0 ? pageSize : 20);

        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, ct);
    }
}
