using FastEndpoints;
using MediatR;
using UserManagement.Contract.Commands;
using UserManagement.Contract.Responses;

namespace UserManagement.API.Endpoints.Users;

public class UpdateUserRolesEndpoint(IMediator mediator) : Endpoint<UpdateUserRolesCommand, UpdateUserRolesResponse>
{
    public override void Configure()
    {
        Put("/users/{Id}/roles");
        Policies("CanManageUsers");
    }

    public override async Task HandleAsync(UpdateUserRolesCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);
        
        if (!response.Success)
        {
            if (response.Message?.Contains("not found in database") == true)
            {
                await Send.NotFoundAsync(ct);
                return;
            }
            ThrowError(response.Message ?? "Failed to update user roles", 400);
            return;
        }
        
        await Send.OkAsync(response, ct);
    }
}
