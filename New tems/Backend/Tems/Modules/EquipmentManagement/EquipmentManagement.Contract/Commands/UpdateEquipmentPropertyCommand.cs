using EquipmentManagement.Contract.Responses;
using MediatR;

namespace EquipmentManagement.Contract.Commands;

public record UpdateEquipmentPropertyCommand(
    string PropertyId,
    string Name,
    string Description,
    string DataType,
    bool Required)
    : IRequest<UpdateEquipmentPropertyResponse>;