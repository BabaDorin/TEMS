using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Contract.Commands;

public record UnassignAssetFromUserCommand(string AssetId) : IRequest<AssetDto>;
