using AssetManagement.Contract.DTOs;
﻿using AssetManagement.Application.Domain;
using AssetManagement.Application.Interfaces;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using MediatR;

public class UpdateAssetPropertyCommandHandler(IAssetPropertyRepository assetPropertyRepository)
    : IRequestHandler<UpdateAssetPropertyCommand, UpdateAssetPropertyResponse>
{
    public async Task<UpdateAssetPropertyResponse> Handle(UpdateAssetPropertyCommand request, CancellationToken ct)
    {
        var existing = await assetPropertyRepository.GetByIdAsync(request.PropertyId, ct);

        if (existing is null)
        {
            return new UpdateAssetPropertyResponse(false, "Property not found.");
        }

        existing.Name = request.Name;
        existing.Description = request.Description;
        existing.DataType = request.DataType;
        existing.Category = request.Category;
        existing.DefaultValidation = request.DefaultValidation != null ? new PropertyValidation
        {
            Type = request.DefaultValidation.Type,
            MaxLength = request.DefaultValidation.MaxLength,
            Pattern = request.DefaultValidation.Pattern,
            Min = request.DefaultValidation.Min,
            Max = request.DefaultValidation.Max,
            Unit = request.DefaultValidation.Unit,
            EnumValues = request.DefaultValidation.EnumValues ?? []
        } : null;
        existing.EnumValues = request.EnumValues;
        existing.Unit = request.Unit;
        existing.UpdatedAt = DateTime.UtcNow;

        var result = await assetPropertyRepository.UpdateAsync(existing, ct);

        return new UpdateAssetPropertyResponse(result, result
            ? "Asset property updated successfully."
            : "Failed to update asset property.");
    }
}


