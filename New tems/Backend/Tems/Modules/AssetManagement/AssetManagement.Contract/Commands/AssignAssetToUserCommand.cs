using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Contract.Commands;

public record AssignAssetToUserCommand(string AssetId, string UserId, string UserName) : IRequest<AssetDto>;
