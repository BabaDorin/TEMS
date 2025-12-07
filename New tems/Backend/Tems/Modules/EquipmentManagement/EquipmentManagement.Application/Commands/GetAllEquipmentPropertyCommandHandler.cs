using EquipmentManagement.Application.Interfaces;
using EquipmentManagement.Contract.Commands;
using EquipmentManagement.Contract.Responses;
using MediatR;

namespace EquipmentManagement.Application.Commands;

public class GetAllEquipmentPropertyCommandHandler(IEquipmentPropertyRepository equipmentPropertyRepository)
    : IRequestHandler<GetAllEquipmentPropertyCommand, GetAllEquipmentPropertyResponse>
{
    public async Task<GetAllEquipmentPropertyResponse> Handle(GetAllEquipmentPropertyCommand request, CancellationToken cancellationToken)
    {
        var domainEntities = await equipmentPropertyRepository.GetAllAsync(cancellationToken);

        var items = domainEntities.Select(entity => new EquipmentPropertyItem(
            PropertyId: entity.PropertyId,
            Name: entity.Name,
            Description: entity.Description,
            DataType: entity.DataType,
            Required: entity.Required,
            DisplayOrder: entity.DisplayOrder,
            CreatedAt: entity.CreatedAt,
            UpdatedAt: entity.UpdatedAt
        )).ToList();

        var response = new GetAllEquipmentPropertyResponse(items);

        return response;
    }
}