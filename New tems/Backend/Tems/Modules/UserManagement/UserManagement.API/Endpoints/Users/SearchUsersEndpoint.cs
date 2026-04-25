using FastEndpoints;
using MediatR;
using UserManagement.Contract.Commands;
using UserManagement.Contract.DTOs;

namespace UserManagement.API.Endpoints.Users;

public class SearchUsersEndpoint(IMediator mediator) : EndpointWithoutRequest<List<UserLookupDto>>
{
    public override void Configure()
    {
        Get("/users/search/by-name");
        Policies("CanOpenOrManageTickets");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var name = Query<string>("name", false);
        var take = Query<int>("take", false);

        var result = await mediator.Send(
            new SearchUsersCommand(
                Name: name,
                Take: take > 0 ? take : 10
            ),
            ct
        );

        await Send.OkAsync(result, ct);
    }
}
