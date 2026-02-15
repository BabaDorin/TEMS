using AssetManagement.Application.Interfaces;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Application.Commands;

public class GetAssetCountsByUsersCommandHandler(IAssetRepository assetRepository)
    : IRequestHandler<GetAssetCountsByUsersCommand, GetAssetCountsByUsersResponse>
{
    public async Task<GetAssetCountsByUsersResponse> Handle(
        GetAssetCountsByUsersCommand request,
        CancellationToken cancellationToken)
    {
        if (request.UserIds.Count == 0)
            return new GetAssetCountsByUsersResponse(true, null, []);

        var counts = await assetRepository.GetAssetCountsByUserIdsAsync(
            request.UserIds,
            cancellationToken);

        return new GetAssetCountsByUsersResponse(true, null, counts);
    }
}
