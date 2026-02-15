using FastEndpoints;
using MediatR;
using UserManagement.Contract.Commands;
using UserManagement.Contract.Responses;

namespace UserManagement.API.Endpoints.Users;

public class CreateUserEndpoint(IMediator mediator) : Endpoint<CreateUserCommand, CreateUserResponse>
{
    public override void Configure()
    {
        Post("/users");
        Policies("CanManageUsers");
    }

    public override async Task HandleAsync(CreateUserCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);
        
        if (!response.Success)
        {
            if (response.Message?.Contains("already exists") == true)
            {
                await Send.CreatedAtAsync<CreateUserEndpoint>(null, response, cancellation: ct);
                return;
            }
            ThrowError(response.Message ?? "Failed to create user", 400);
            return;
        }
        
        await Send.CreatedAtAsync<GetAllUsersEndpoint>(null, response, cancellation: ct);
    }
}
