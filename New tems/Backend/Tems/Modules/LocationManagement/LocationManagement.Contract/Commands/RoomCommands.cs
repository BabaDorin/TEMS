using LocationManagement.Contract.Responses;
using MediatR;

namespace LocationManagement.Contract.Commands;

public record CreateRoomCommand(
    string BuildingId,
    string Name,
    string? RoomNumber,
    string FloorLabel,
    string Type,
    int Capacity,
    double? Area,
    string Status,
    string? Description,
    string CreatedBy)
    : IRequest<CreateRoomResponse>;

public record UpdateRoomCommand(
    string Id,
    string BuildingId,
    string Name,
    string? RoomNumber,
    string FloorLabel,
    string Type,
    int Capacity,
    double? Area,
    string Status,
    string? Description,
    string UpdatedBy)
    : IRequest<UpdateRoomResponse>;

public record DeleteRoomCommand(string Id) : IRequest<DeleteRoomResponse>;

public record GetRoomByIdCommand(string Id) : IRequest<GetRoomByIdResponse>;

public record GetAllRoomsCommand(string? SiteId = null, string? BuildingId = null) : IRequest<GetAllRoomsResponse>;

public record GetLocationHierarchyCommand() : IRequest<GetLocationHierarchyResponse>;
