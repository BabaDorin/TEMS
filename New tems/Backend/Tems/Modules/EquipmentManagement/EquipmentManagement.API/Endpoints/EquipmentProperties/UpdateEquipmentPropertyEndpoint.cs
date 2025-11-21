using EquipmentManagement.Contract.Commands;
using EquipmentManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace EquipmentManagement.API.Endpoints.EquipmentProperties;

public class UpdateEquipmentPropertyEndpoint(IMediator mediator) : Endpoint<UpdateEquipmentPropertyCommand, UpdateEquipmentPropertyResponse>
{
    public override void Configure()
    {
        Put("/equipment-properties/{propertyId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UpdateEquipmentPropertyCommand command, CancellationToken ct)
    {
        command = command with { PropertyId = Route<string>("propertyId") };
        var response = await mediator.Send(command, ct);

        if (!response.Success)
        {
            ThrowError(response.Message);
        }
    }
}