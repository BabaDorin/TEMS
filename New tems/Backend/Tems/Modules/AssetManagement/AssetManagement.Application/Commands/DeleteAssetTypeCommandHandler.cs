using AssetManagement.Application.Interfaces;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Application.Commands;

public class DeleteAssetTypeCommandHandler(IAssetTypeRepository assetTypeRepository) 
    : IRequestHandler<DeleteAssetTypeCommand, DeleteAssetTypeResponse>
{
    public async Task<DeleteAssetTypeResponse> Handle(DeleteAssetTypeCommand request, CancellationToken cancellationToken)
    {
        var success = await assetTypeRepository.DeleteAsync(request.Id, cancellationToken);

        return new DeleteAssetTypeResponse(success);
    }
}
