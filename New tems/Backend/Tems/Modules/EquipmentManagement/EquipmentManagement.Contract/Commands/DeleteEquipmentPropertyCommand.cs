using EquipmentManagement.Contract.Responses;
using MediatR;

namespace EquipmentManagement.Contract.Commands;

public record DeleteEquipmentPropertyCommand(
    string PropertyId)
    : IRequest<DeleteEquipmentPropertyResponse>;