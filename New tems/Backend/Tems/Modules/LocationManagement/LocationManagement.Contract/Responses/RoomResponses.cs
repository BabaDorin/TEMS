using LocationManagement.Contract.DTOs;

namespace LocationManagement.Contract.Responses;

public record CreateRoomResponse(bool Success, string? Message, RoomDto? Data);
public record UpdateRoomResponse(bool Success, string? Message, RoomDto? Data);
public record DeleteRoomResponse(bool Success, string? Message);
public record GetRoomByIdResponse(bool Success, string? Message, RoomDto? Data);
public record GetAllRoomsResponse(
    bool Success,
    string? Message,
    List<RoomDto> Data,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages
);
public record GetLocationHierarchyResponse(bool Success, string? Message, List<LocationHierarchyDto> Data);
