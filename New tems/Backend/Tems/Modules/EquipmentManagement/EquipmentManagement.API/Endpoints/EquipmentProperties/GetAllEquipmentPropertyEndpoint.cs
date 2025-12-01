using EquipmentManagement.Contract.Commands;
using EquipmentManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace EquipmentManagement.API.Endpoints.EquipmentProperties;

public class GetAllEquipmentPropertyEndpoint(IMediator mediator)
    : EndpointWithoutRequest<GetAllEquipmentPropertyResponse>
{
    public override void Configure()
    {
        Get("/equipment-properties");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var command = new GetAllEquipmentPropertyCommand();
        var response = await mediator.Send(command, ct);
    }
}