using EquipmentManagement.Contract.Responses;
using MediatR;

namespace EquipmentManagement.Contract.Commands;

public record GetByIdEquipmentPropertyCommand(string PropertyId) : IRequest<GetByIdEquipmentPropertyResponse>;