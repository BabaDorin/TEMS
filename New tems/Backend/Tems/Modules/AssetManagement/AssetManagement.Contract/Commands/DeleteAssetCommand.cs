using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Contract.Commands;

public record DeleteAssetCommand(string Id) : IRequest<DeleteAssetResponse>;
