namespace EquipmentManagement.Contract.Responses;

public record GetByIdEquipmentPropertyResponse(
    string PropertyId,
    string Name,
    string Description,
    string DataType,
    bool Required,
    int DisplayOrder,
    DateTime CreatedAt,
    DateTime UpdatedAt);