using LocationManagement.Contract.Responses;
using MediatR;

namespace LocationManagement.Contract.Commands;

public record CreateSiteCommand(
    string Name,
    string Code,
    string Timezone,
    bool IsActive,
    string CreatedBy) 
    : IRequest<CreateSiteResponse>;

public record UpdateSiteCommand(
    string Id,
    string Name,
    string Code,
    string Timezone,
    bool IsActive,
    string UpdatedBy)
    : IRequest<UpdateSiteResponse>;

public record DeleteSiteCommand(string Id) : IRequest<DeleteSiteResponse>;

public record GetSiteByIdCommand(string Id) : IRequest<GetSiteByIdResponse>;

public record GetAllSitesCommand() : IRequest<GetAllSitesResponse>;
