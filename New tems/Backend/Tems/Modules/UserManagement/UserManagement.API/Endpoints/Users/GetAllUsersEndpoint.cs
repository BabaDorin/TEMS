using FastEndpoints;
using MediatR;
using UserManagement.Contract.Commands;
using UserManagement.Contract.Responses;

namespace UserManagement.API.Endpoints.Users;

public class GetAllUsersEndpoint(IMediator mediator) : EndpointWithoutRequest<GetAllUsersResponse>
{
    public override void Configure()
    {
        Get("/users");
        Policies("CanManageUsers");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var pageNumber = Query<int>("pageNumber", false);
        var pageSize = Query<int>("pageSize", false);

        var command = new GetAllUsersCommand(
            PageNumber: pageNumber > 0 ? pageNumber : 1,
            PageSize: pageSize > 0 ? pageSize : 50
        );

        var result = await mediator.Send(command, ct);
        await Send.OkAsync(result, ct);
    }
}
