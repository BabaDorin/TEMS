using LocationManagement.Contract.DTOs;

namespace LocationManagement.Contract.Responses;

public record CreateSiteResponse(bool Success, string? Message, SiteDto? Data);
public record UpdateSiteResponse(bool Success, string? Message, SiteDto? Data);
public record DeleteSiteResponse(bool Success, string? Message);
public record GetSiteByIdResponse(bool Success, string? Message, SiteDto? Data);
public record GetAllSitesResponse(bool Success, string? Message, List<SiteDto> Data);
