using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Contract.Commands;

public record GetAllAssetDefinitionCommand(bool IncludeArchived = false) : IRequest<GetAllAssetDefinitionResponse>;
