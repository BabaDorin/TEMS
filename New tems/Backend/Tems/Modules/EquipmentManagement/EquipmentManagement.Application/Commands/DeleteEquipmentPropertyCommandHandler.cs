using EquipmentManagement.Application.Interfaces;
using EquipmentManagement.Contract.Commands;
using EquipmentManagement.Contract.Responses;
using MediatR;

namespace EquipmentManagement.Application.Commands;

public class DeleteEquipmentPropertyCommandHandler(IEquipmentPropertyRepository equipmentPropertyRepository)
    : IRequestHandler<DeleteEquipmentPropertyCommand, DeleteEquipmentPropertyResponse>
{
    public async Task<DeleteEquipmentPropertyResponse> Handle(DeleteEquipmentPropertyCommand request, CancellationToken cancellationToken)
    {
        var propertyExists = await equipmentPropertyRepository.GetByIdAsync(request.PropertyId, cancellationToken);
        if (propertyExists == null)
        {
            return new DeleteEquipmentPropertyResponse(false, "Equipment property not found.");
        }

        var deleted = await equipmentPropertyRepository.DeleteAsync(request.PropertyId, cancellationToken);

        return new DeleteEquipmentPropertyResponse(deleted, deleted ? "Equipment property deleted successfully." : "Failed to delete equipment property.");
    }
}