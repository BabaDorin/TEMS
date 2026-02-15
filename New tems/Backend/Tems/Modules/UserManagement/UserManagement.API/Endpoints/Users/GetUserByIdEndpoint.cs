using FastEndpoints;
using MediatR;
using UserManagement.Contract.Commands;
using UserManagement.Contract.DTOs;

namespace UserManagement.API.Endpoints.Users;

public class GetUserByIdEndpoint(IMediator mediator) : EndpointWithoutRequest<UserDto>
{
    public override void Configure()
    {
        Get("/users/{id}");
        Policies("CanManageUsers");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<string>("id");
        
        if (string.IsNullOrEmpty(id))
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var result = await mediator.Send(new GetUserByIdCommand(id), ct);
        
        if (result == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }
        
        await Send.OkAsync(result, ct);
    }
}
