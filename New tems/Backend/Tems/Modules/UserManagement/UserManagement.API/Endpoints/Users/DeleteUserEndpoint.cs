using FastEndpoints;
using MediatR;
using UserManagement.Contract.Commands;
using UserManagement.Contract.Responses;

namespace UserManagement.API.Endpoints.Users;

public class DeleteUserEndpoint(IMediator mediator) : EndpointWithoutRequest<DeleteUserResponse>
{
    public override void Configure()
    {
        Delete("/users/{Id}");
        Policies("CanManageUsers");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<string>("Id")!;
        var callerKeycloakId = User.FindFirst("sub")?.Value;
        var command = new DeleteUserCommand(id, callerKeycloakId);

        var response = await mediator.Send(command, ct);
        
        if (!response.Success)
        {
            if (response.Message?.Contains("not found") == true)
            {
                await Send.NotFoundAsync(ct);
                return;
            }
            ThrowError(response.Message ?? "Failed to delete user", 400);
            return;
        }
        
        await Send.OkAsync(response, ct);
    }
}
