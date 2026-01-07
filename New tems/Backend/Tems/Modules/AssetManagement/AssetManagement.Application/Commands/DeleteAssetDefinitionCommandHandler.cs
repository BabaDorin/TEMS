using AssetManagement.Application.Interfaces;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Application.Commands;

public class DeleteAssetDefinitionCommandHandler(IAssetDefinitionRepository assetDefinitionRepository) 
    : IRequestHandler<DeleteAssetDefinitionCommand, DeleteAssetDefinitionResponse>
{
    public async Task<DeleteAssetDefinitionResponse> Handle(DeleteAssetDefinitionCommand request, CancellationToken cancellationToken)
    {
        var success = await assetDefinitionRepository.DeleteAsync(request.Id, cancellationToken);

        return new DeleteAssetDefinitionResponse(success);
    }
}
