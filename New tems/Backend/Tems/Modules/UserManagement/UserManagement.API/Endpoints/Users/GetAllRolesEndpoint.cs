using FastEndpoints;
using MediatR;
using UserManagement.Contract.Commands;
using UserManagement.Contract.Responses;

namespace UserManagement.API.Endpoints.Users;

public class GetAllRolesEndpoint(IMediator mediator) : EndpointWithoutRequest<GetAllRolesResponse>
{
    public override void Configure()
    {
        Get("/roles");
        Policies("CanManageUsers");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var command = new GetAllRolesCommand();
        var result = await mediator.Send(command, ct);
        await Send.OkAsync(result, ct);
    }
}
