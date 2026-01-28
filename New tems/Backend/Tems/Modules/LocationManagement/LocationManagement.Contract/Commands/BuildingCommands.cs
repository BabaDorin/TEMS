using LocationManagement.Contract.Responses;
using MediatR;

namespace LocationManagement.Contract.Commands;

public record CreateBuildingCommand(
    string SiteId,
    string Name,
    string AddressLine,
    string ManagerContact,
    string CreatedBy)
    : IRequest<CreateBuildingResponse>;

public record UpdateBuildingCommand(
    string Id,
    string SiteId,
    string Name,
    string AddressLine,
    string ManagerContact,
    string UpdatedBy)
    : IRequest<UpdateBuildingResponse>;

public record DeleteBuildingCommand(string Id) : IRequest<DeleteBuildingResponse>;

public record GetBuildingByIdCommand(string Id) : IRequest<GetBuildingByIdResponse>;

public record GetAllBuildingsCommand(string? SiteId = null) : IRequest<GetAllBuildingsResponse>;
