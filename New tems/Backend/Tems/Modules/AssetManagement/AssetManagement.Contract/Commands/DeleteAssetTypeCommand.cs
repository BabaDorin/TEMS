using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Contract.Commands;

public record DeleteAssetTypeCommand(string Id) : IRequest<DeleteAssetTypeResponse>;
