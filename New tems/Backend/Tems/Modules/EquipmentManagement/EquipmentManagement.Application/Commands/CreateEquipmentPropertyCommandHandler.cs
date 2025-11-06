using EquipmentManagement.Application.Domain;
using EquipmentManagement.Application.Interfaces;
using EquipmentManagement.Contract.Commands;
using EquipmentManagement.Contract.Responses;
using MediatR;

namespace EquipmentManagement.Application.Commands;

public class CreateEquipmentPropertyCommandHandler(IEquipmentPropertyRepository equipmentPropertyRepository) 
    : IRequestHandler<CreateEquipmentPropertyCommand, CreateEquipmentPropertyResponse>
{
    public async Task<CreateEquipmentPropertyResponse> Handle(CreateEquipmentPropertyCommand request, CancellationToken cancellationToken)
    {
        // Step 1: Validate the request
        // TODO
        
        // Step 2: Create domain entity, based on request data
        var domainEntity = new EquipmentProperty
        {
            PropertyId = Guid.NewGuid().ToString(),
            Name = request.Name,
            Description = request.Description,
            DataType = request.DataType,
            Required = request.Required,
            Validation = null,
            DisplayOrder = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        // Step 3: Create the entity using repository
        await equipmentPropertyRepository.CreateAsync(domainEntity, cancellationToken);
        
        // Step 4: Return the response back to the client
        var response = new CreateEquipmentPropertyResponse(domainEntity.PropertyId);
        
        return response;
    }
}