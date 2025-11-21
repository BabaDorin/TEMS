using EquipmentManagement.Application.Interfaces;
using EquipmentManagement.Contract.Commands;
using EquipmentManagement.Contract.Responses;
using MediatR;

public class UpdateEquipmentPropertyCommandHandler(IEquipmentPropertyRepository equipmentPropertyRepository)
    : IRequestHandler<UpdateEquipmentPropertyCommand, UpdateEquipmentPropertyResponse>
{
    public async Task<UpdateEquipmentPropertyResponse> Handle(UpdateEquipmentPropertyCommand request, CancellationToken ct)
    {
        var existing = await equipmentPropertyRepository.GetByIdAsync(request.PropertyId, ct);

        if (existing is null)
        {
            return new UpdateEquipmentPropertyResponse(false, "Property not found.");
        }

        existing.Name = request.Name;
        existing.Description = request.Description;
        existing.DataType = request.DataType;
        existing.Required = request.Required;
        existing.UpdatedAt = DateTime.UtcNow;

        var result = await equipmentPropertyRepository.UpdateAsync(existing, ct);

        return new UpdateEquipmentPropertyResponse(result, result
            ? "Equipment property updated successfully."
            : "Failed to update equipment property.");
    }
}


