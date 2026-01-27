using LocationManagement.Contract.DTOs;
using LocationManagement.Application.Domain;
using LocationManagement.Application.Interfaces;
using LocationManagement.Contract.Commands;
using LocationManagement.Contract.Responses;
using MediatR;
using Tems.Common.Tenant;

namespace LocationManagement.Application.Commands;

public class CreateSiteCommandHandler(ISiteRepository siteRepository, ITenantContext tenantContext) 
    : IRequestHandler<CreateSiteCommand, CreateSiteResponse>
{
    public async Task<CreateSiteResponse> Handle(CreateSiteCommand request, CancellationToken cancellationToken)
    {
        var tenantId = tenantContext.TenantId ?? throw new InvalidOperationException("Tenant context is required");

        var domainEntity = new Site
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name.Trim(),
            Code = request.Code.Trim().ToUpperInvariant(),
            Timezone = request.Timezone,
            IsActive = request.IsActive,
            TenantId = tenantId,
            CreatedBy = request.CreatedBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await siteRepository.CreateAsync(domainEntity, cancellationToken);

        var dto = new SiteDto(
            domainEntity.Id,
            domainEntity.Name,
            domainEntity.Code,
            domainEntity.Timezone,
            domainEntity.IsActive,
            domainEntity.TenantId,
            domainEntity.CreatedAt,
            domainEntity.UpdatedAt
        );

        return new CreateSiteResponse(true, "Site created successfully", dto);
    }
}

public class GetAllSitesCommandHandler(ISiteRepository siteRepository, ITenantContext tenantContext)
    : IRequestHandler<GetAllSitesCommand, GetAllSitesResponse>
{
    public async Task<GetAllSitesResponse> Handle(GetAllSitesCommand request, CancellationToken cancellationToken)
    {
        var tenantId = tenantContext.TenantId ?? throw new InvalidOperationException("Tenant context is required");
        var sites = await siteRepository.GetAllAsync(tenantId, cancellationToken);

        var siteDtos = sites.Select(s => new SiteDto(
            s.Id,
            s.Name,
            s.Code,
            s.Timezone,
            s.IsActive,
            s.TenantId,
            s.CreatedAt,
            s.UpdatedAt
        )).ToList();

        return new GetAllSitesResponse(true, null, siteDtos);
    }
}

public class GetSiteByIdCommandHandler(ISiteRepository siteRepository, ITenantContext tenantContext)
    : IRequestHandler<GetSiteByIdCommand, GetSiteByIdResponse>
{
    public async Task<GetSiteByIdResponse> Handle(GetSiteByIdCommand request, CancellationToken cancellationToken)
    {
        var tenantId = tenantContext.TenantId ?? throw new InvalidOperationException("Tenant context is required");
        var site = await siteRepository.GetByIdAsync(request.Id, tenantId, cancellationToken);

        if (site == null)
            return new GetSiteByIdResponse(false, "Site not found", null);

        var dto = new SiteDto(
            site.Id,
            site.Name,
            site.Code,
            site.Timezone,
            site.IsActive,
            site.TenantId,
            site.CreatedAt,
            site.UpdatedAt
        );

        return new GetSiteByIdResponse(true, null, dto);
    }
}

public class UpdateSiteCommandHandler(ISiteRepository siteRepository, ITenantContext tenantContext)
    : IRequestHandler<UpdateSiteCommand, UpdateSiteResponse>
{
    public async Task<UpdateSiteResponse> Handle(UpdateSiteCommand request, CancellationToken cancellationToken)
    {
        var tenantId = tenantContext.TenantId ?? throw new InvalidOperationException("Tenant context is required");
        var existing = await siteRepository.GetByIdAsync(request.Id, tenantId, cancellationToken);

        if (existing == null)
            return new UpdateSiteResponse(false, "Site not found", null);

        existing.Name = request.Name.Trim();
        existing.Code = request.Code.Trim().ToUpperInvariant();
        existing.Timezone = request.Timezone;
        existing.IsActive = request.IsActive;
        existing.UpdatedBy = request.UpdatedBy;
        existing.UpdatedAt = DateTime.UtcNow;

        await siteRepository.UpdateAsync(existing, cancellationToken);

        var dto = new SiteDto(
            existing.Id,
            existing.Name,
            existing.Code,
            existing.Timezone,
            existing.IsActive,
            existing.TenantId,
            existing.CreatedAt,
            existing.UpdatedAt
        );

        return new UpdateSiteResponse(true, "Site updated successfully", dto);
    }
}

public class DeleteSiteCommandHandler(ISiteRepository siteRepository, ITenantContext tenantContext)
    : IRequestHandler<DeleteSiteCommand, DeleteSiteResponse>
{
    public async Task<DeleteSiteResponse> Handle(DeleteSiteCommand request, CancellationToken cancellationToken)
    {
        var tenantId = tenantContext.TenantId ?? throw new InvalidOperationException("Tenant context is required");
        var deleted = await siteRepository.DeleteAsync(request.Id, tenantId, cancellationToken);

        return deleted 
            ? new DeleteSiteResponse(true, "Site deleted successfully")
            : new DeleteSiteResponse(false, "Site not found");
    }
}
