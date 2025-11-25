using EquipmentManagement.Contract.Commands;
using EquipmentManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace EquipmentManagement.API.Endpoints.EquipmentProperties;
 
public class DeleteEquipmentPropertyEndpoint(IMediator mediator) : Endpoint<DeleteEquipmentPropertyCommand, DeleteEquipmentPropertyResponse>
{
    public override void Configure()
    {
        Delete("/equipment-properties/{propertyId}");
        Policies("CanManageEntities");
    }

    public override async Task HandleAsync(DeleteEquipmentPropertyCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);

        if (!response.Success)
        {
            ThrowError(response.Message);
        }
    }
}
