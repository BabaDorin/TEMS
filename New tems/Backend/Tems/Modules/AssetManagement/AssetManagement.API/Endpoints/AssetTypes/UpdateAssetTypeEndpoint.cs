using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using AssetManagement.Application.Exceptions;
using FastEndpoints;
using MediatR;

namespace AssetManagement.API.Endpoints.AssetTypes;

public class UpdateAssetTypeEndpoint(IMediator mediator) : Endpoint<UpdateAssetTypeCommand, UpdateAssetTypeResponse>
{
    public override void Configure()
    {
        Put("/asset-type/{Id}");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(UpdateAssetTypeCommand command, CancellationToken ct)
    {
        try
        {
            var response = await mediator.Send(command, ct);
            await Send.OkAsync(response, cancellation: ct);
        }
        catch (DuplicateAssetTypeNameException ex)
        {
            ThrowError(ex.Message, 409);
        }
    }
}
