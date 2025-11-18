using EquipmentManagement.Contract.Commands;
using EquipmentManagement.Contract.Responses;
using Example.Contract.Features;
using FastEndpoints;
using MediatR;

namespace EquipmentManagement.API.Endpoints.EquipmentProperties;

public class CreateEquipmentPropertyEndpoint(IMediator mediator) : Endpoint<CreateEquipmentPropertyCommand, CreateEquipmentPropertyResponse>
{
    public override void Configure()
    {
        Post("/equipment-properties");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateEquipmentPropertyCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);

        Console.WriteLine(response.PropertyId);
    }
}