using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Contract.Commands;

public record DeleteAssetPropertyCommand(
    string PropertyId)
    : IRequest<DeleteAssetPropertyResponse>;