using AssetManagement.Contract.DTOs;
using AssetManagement.Application.Domain;
using AssetManagement.Application.Interfaces;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Application.Commands;

public class CreateAssetPropertyCommandHandler(IAssetPropertyRepository assetPropertyRepository) 
    : IRequestHandler<CreateAssetPropertyCommand, CreateAssetPropertyResponse>
{
    public async Task<CreateAssetPropertyResponse> Handle(CreateAssetPropertyCommand request, CancellationToken cancellationToken)
    {
        var domainEntity = new AssetProperty
        {
            PropertyId = Guid.NewGuid().ToString(),
            Name = request.Name,
            Description = request.Description,
            DataType = request.DataType,
            Category = request.Category,
            DefaultValidation = request.DefaultValidation != null ? new PropertyValidation
            {
                Type = request.DefaultValidation.Type,
                MaxLength = request.DefaultValidation.MaxLength,
                Pattern = request.DefaultValidation.Pattern,
                Min = request.DefaultValidation.Min,
                Max = request.DefaultValidation.Max,
                Unit = request.DefaultValidation.Unit,
                EnumValues = request.DefaultValidation.EnumValues ?? []
            } : null,
            EnumValues = request.EnumValues,
            Unit = request.Unit,
            CreatedBy = request.CreatedBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await assetPropertyRepository.CreateAsync(domainEntity, cancellationToken);

        return new CreateAssetPropertyResponse(domainEntity.PropertyId);
    }
}