using EquipmentManagement.Contract.Responses;
using MediatR;

namespace EquipmentManagement.Contract.Commands;

public record CreateEquipmentPropertyCommand(
    string Name,
    string Description,
    string DataType,
    bool Required) 
    : IRequest<CreateEquipmentPropertyResponse>;