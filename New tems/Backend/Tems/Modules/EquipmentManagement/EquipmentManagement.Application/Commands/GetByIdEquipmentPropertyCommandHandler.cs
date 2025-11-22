using EquipmentManagement.Application.Interfaces;
using EquipmentManagement.Contract.Commands;
using EquipmentManagement.Contract.Responses;
using MediatR;

namespace EquipmentManagement.Application.Commands;

public class GetByIdEquipmentPropertyCommandHandler(IEquipmentPropertyRepository equipmentPropertyRepository) : IRequestHandler<GetByIdEquipmentPropertyCommand, GetByIdEquipmentPropertyResponse>
{
    public async Task<GetByIdEquipmentPropertyResponse> Handle(GetByIdEquipmentPropertyCommand request, CancellationToken cancellationToken)
    {
        var domainEntity = await equipmentPropertyRepository.GetByIdAsync(request.PropertyId, cancellationToken);

        if (domainEntity == null)
        {
            throw new KeyNotFoundException($"Equipment property with ID '{request.PropertyId}' not found.");
        }

        var response = new GetByIdEquipmentPropertyResponse(
            PropertyId: domainEntity.PropertyId,
            Name: domainEntity.Name,
            Description: domainEntity.Description,
            DataType: domainEntity.DataType,
            Required: domainEntity.Required,
            DisplayOrder: domainEntity.DisplayOrder,
            CreatedAt: domainEntity.CreatedAt,
            UpdatedAt: domainEntity.UpdatedAt
        );

        return response;
    }
}