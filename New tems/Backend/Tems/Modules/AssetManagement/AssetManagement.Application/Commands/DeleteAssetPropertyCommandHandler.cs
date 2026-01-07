using AssetManagement.Application.Interfaces;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Application.Commands;

public class DeleteAssetPropertyCommandHandler(IAssetPropertyRepository assetPropertyRepository)
    : IRequestHandler<DeleteAssetPropertyCommand, DeleteAssetPropertyResponse>
{
    public async Task<DeleteAssetPropertyResponse> Handle(DeleteAssetPropertyCommand request, CancellationToken cancellationToken)
    {
        var propertyExists = await assetPropertyRepository.GetByIdAsync(request.PropertyId, cancellationToken);
        if (propertyExists == null)
        {
            return new DeleteAssetPropertyResponse(false, "Equipment property not found.");
        }

        var deleted = await assetPropertyRepository.DeleteAsync(request.PropertyId, cancellationToken);

        return new DeleteAssetPropertyResponse(deleted, deleted ? "Equipment property deleted successfully." : "Failed to delete asset property.");
    }
}