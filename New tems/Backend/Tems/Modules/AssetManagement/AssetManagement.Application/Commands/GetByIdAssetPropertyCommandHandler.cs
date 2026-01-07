using AssetManagement.Contract.DTOs;
﻿using AssetManagement.Application.Interfaces;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Application.Commands;

public class GetByIdAssetPropertyCommandHandler(IAssetPropertyRepository assetPropertyRepository) : IRequestHandler<GetByIdAssetPropertyCommand, GetByIdAssetPropertyResponse>
{
    public async Task<GetByIdAssetPropertyResponse> Handle(GetByIdAssetPropertyCommand request, CancellationToken cancellationToken)
    {
        var domainEntity = await assetPropertyRepository.GetByIdAsync(request.PropertyId, cancellationToken);

        if (domainEntity == null)
        {
            return new GetByIdAssetPropertyResponse(null);
        }

        var dto = new AssetPropertyDto(
            PropertyId: domainEntity.PropertyId,
            Name: domainEntity.Name,
            Description: domainEntity.Description,
            DataType: domainEntity.DataType,
            Category: domainEntity.Category,
            DefaultValidation: domainEntity.DefaultValidation != null ? new PropertyValidationDto(
                domainEntity.DefaultValidation.Type,
                domainEntity.DefaultValidation.MaxLength,
                domainEntity.DefaultValidation.Pattern,
                domainEntity.DefaultValidation.Min,
                domainEntity.DefaultValidation.Max,
                domainEntity.DefaultValidation.Unit,
                domainEntity.DefaultValidation.EnumValues
            ) : null,
            EnumValues: domainEntity.EnumValues,
            Unit: domainEntity.Unit,
            CreatedAt: domainEntity.CreatedAt,
            UpdatedAt: domainEntity.UpdatedAt,
            CreatedBy: domainEntity.CreatedBy
        );

        return new GetByIdAssetPropertyResponse(dto);
    }
}