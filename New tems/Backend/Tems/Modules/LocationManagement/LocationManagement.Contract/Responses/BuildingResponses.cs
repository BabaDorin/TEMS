using LocationManagement.Contract.DTOs;

namespace LocationManagement.Contract.Responses;

public record CreateBuildingResponse(bool Success, string? Message, BuildingDto? Data);
public record UpdateBuildingResponse(bool Success, string? Message, BuildingDto? Data);
public record DeleteBuildingResponse(bool Success, string? Message);
public record GetBuildingByIdResponse(bool Success, string? Message, BuildingDto? Data);
public record GetAllBuildingsResponse(bool Success, string? Message, List<BuildingDto> Data);
