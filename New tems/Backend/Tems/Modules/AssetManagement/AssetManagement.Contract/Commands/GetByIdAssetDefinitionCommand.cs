using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Contract.Commands;

public record GetByIdAssetDefinitionCommand(string Id) : IRequest<GetByIdAssetDefinitionResponse>;
