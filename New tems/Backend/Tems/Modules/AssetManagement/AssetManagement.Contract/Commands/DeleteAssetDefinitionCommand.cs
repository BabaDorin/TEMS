using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Contract.Commands;

public record DeleteAssetDefinitionCommand(string Id) : IRequest<DeleteAssetDefinitionResponse>;
