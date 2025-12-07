namespace EquipmentManagement.Contract.Responses;

public record GetAllEquipmentPropertyResponse(List<EquipmentPropertyItem> Properties);

public record EquipmentPropertyItem(
    string PropertyId,
    string Name,
    string Description,
    string DataType,
    bool Required,
    int DisplayOrder,
    DateTime CreatedAt,
    DateTime UpdatedAt);