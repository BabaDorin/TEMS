using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Contract.Commands;

public record AssignAssetToRoomCommand(string AssetId, string RoomId) : IRequest<AssetDto>;
