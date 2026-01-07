using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Contract.Commands;

public record GetByIdAssetPropertyCommand(string PropertyId) : IRequest<GetByIdAssetPropertyResponse>;