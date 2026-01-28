using FastEndpoints;
using MediatR;
using UserManagement.Application.Queries;
using UserManagement.Contract.Responses;
using System.Security.Claims;

namespace UserManagement.API.Endpoints;

public class GetProfileEndpoint(IMediator mediator) : EndpointWithoutRequest<GetProfileResponse>
{
    public override void Configure()
    {
        Get("/profile");
        AllowAnonymous(); // Will be changed to require authentication after testing
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        // Extract user information from JWT claims
        var identityProviderId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? User.FindFirst("sub")?.Value;
        
        var name = User.FindFirst("name")?.Value 
            ?? $"{User.FindFirst("given_name")?.Value} {User.FindFirst("family_name")?.Value}".Trim();
        
        var email = User.FindFirst(ClaimTypes.Email)?.Value 
            ?? User.FindFirst("email")?.Value;
        
        var avatarUrl = User.FindFirst("picture")?.Value 
            ?? User.FindFirst("avatar")?.Value;

        if (string.IsNullOrEmpty(identityProviderId))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        if (string.IsNullOrEmpty(name))
        {
            name = User.FindFirst("preferred_username")?.Value ?? "Unknown User";
        }

        if (string.IsNullOrEmpty(email))
        {
            email = "no-email@unknown.local";
        }

        var query = new GetOrCreateProfileQuery(
            identityProviderId,
            name,
            email,
            avatarUrl
        );

        var response = await mediator.Send(query, ct);

        await Send.OkAsync(response, cancellation: ct);
    }
}
