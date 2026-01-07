using AssetManagement.Application.Interfaces;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Application.Commands;

public class DeleteAssetCommandHandler(IAssetRepository assetRepository) 
    : IRequestHandler<DeleteAssetCommand, DeleteAssetResponse>
{
    public async Task<DeleteAssetResponse> Handle(DeleteAssetCommand request, CancellationToken cancellationToken)
    {
        var success = await assetRepository.DeleteAsync(request.Id, cancellationToken);

        return new DeleteAssetResponse(success);
    }
}
