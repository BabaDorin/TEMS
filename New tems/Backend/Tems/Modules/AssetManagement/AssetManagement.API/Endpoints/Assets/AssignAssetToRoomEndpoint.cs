using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace AssetManagement.API.Endpoints.Assets;

public record AssignAssetToRoomRequest(string RoomId);

public class AssignAssetToRoomEndpoint(IMediator mediator) : Endpoint<AssignAssetToRoomRequest, AssetDto>
{
    public override void Configure()
    {
        Put("/asset/{AssetId}/room");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(AssignAssetToRoomRequest request, CancellationToken ct)
    {
        var assetId = Route<string>("AssetId") ?? throw new ArgumentException("Asset ID is required");
        var command = new AssignAssetToRoomCommand(assetId, request.RoomId);
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
