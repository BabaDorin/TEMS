using AssetManagement.Application.Interfaces;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Application.Commands;

public class GetAssetCountsByLocationsCommandHandler(IAssetRepository assetRepository)
    : IRequestHandler<GetAssetCountsByLocationsCommand, GetAssetCountsByLocationsResponse>
{
    public async Task<GetAssetCountsByLocationsResponse> Handle(
        GetAssetCountsByLocationsCommand request, 
        CancellationToken cancellationToken)
    {
        if (request.LocationIds.Count == 0)
            return new GetAssetCountsByLocationsResponse(true, null, []);

        var counts = await assetRepository.GetAssetCountsByLocationIdsAsync(
            request.LocationIds, 
            cancellationToken);

        return new GetAssetCountsByLocationsResponse(true, null, counts);
    }
}
