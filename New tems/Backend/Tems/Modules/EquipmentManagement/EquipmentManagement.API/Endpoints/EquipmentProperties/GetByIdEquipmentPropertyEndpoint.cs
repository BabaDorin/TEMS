using EquipmentManagement.Contract.Commands;
using EquipmentManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace EquipmentManagement.API.Endpoints.EquipmentProperties;

public class GetByIdEquipmentPropertyEndpoint(IMediator mediator) : Endpoint<GetByIdEquipmentPropertyCommand, GetByIdEquipmentPropertyResponse>
{
    public override void Configure()
    {
        Get("/equipment-properties/{propertyId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetByIdEquipmentPropertyCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);
    }
}