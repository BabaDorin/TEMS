using LocationManagement.Contract.DTOs;
using LocationManagement.Application.Domain;
using LocationManagement.Application.Interfaces;
using LocationManagement.Contract.Commands;
using LocationManagement.Contract.Responses;
using MediatR;
using Tems.Common.Notifications;
using Tems.Common.Tenant;

namespace LocationManagement.Application.Commands;

public class CreateBuildingCommandHandler(IBuildingRepository buildingRepository, ITenantContext tenantContext) 
    : IRequestHandler<CreateBuildingCommand, CreateBuildingResponse>
{
    public async Task<CreateBuildingResponse> Handle(CreateBuildingCommand request, CancellationToken cancellationToken)
    {
        var tenantId = tenantContext.TenantId ?? throw new InvalidOperationException("Tenant context is required");

        var domainEntity = new Building
        {
            Id = Guid.NewGuid().ToString(),
            SiteId = request.SiteId,
            Name = request.Name.Trim(),
            AddressLine = request.AddressLine,
            ManagerContact = request.ManagerContact,
            TenantId = tenantId,
            CreatedBy = request.CreatedBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await buildingRepository.CreateAsync(domainEntity, cancellationToken);

        var dto = new BuildingDto(
            domainEntity.Id,
            domainEntity.SiteId,
            domainEntity.Name,
            domainEntity.AddressLine,
            domainEntity.ManagerContact,
            domainEntity.TenantId,
            domainEntity.CreatedAt,
            domainEntity.UpdatedAt
        );

        return new CreateBuildingResponse(true, "Building created successfully", dto);
    }
}

public class GetAllBuildingsCommandHandler(IBuildingRepository buildingRepository, ITenantContext tenantContext)
    : IRequestHandler<GetAllBuildingsCommand, GetAllBuildingsResponse>
{
    public async Task<GetAllBuildingsResponse> Handle(GetAllBuildingsCommand request, CancellationToken cancellationToken)
    {
        var tenantId = tenantContext.TenantId ?? throw new InvalidOperationException("Tenant context is required");
        var buildings = await buildingRepository.GetAllAsync(tenantId, request.SiteId, cancellationToken);

        var buildingDtos = buildings.Select(b => new BuildingDto(
            b.Id,
            b.SiteId,
            b.Name,
            b.AddressLine,
            b.ManagerContact,
            b.TenantId,
            b.CreatedAt,
            b.UpdatedAt
        )).ToList();

        return new GetAllBuildingsResponse(true, null, buildingDtos);
    }
}

public class GetBuildingByIdCommandHandler(IBuildingRepository buildingRepository, ITenantContext tenantContext)
    : IRequestHandler<GetBuildingByIdCommand, GetBuildingByIdResponse>
{
    public async Task<GetBuildingByIdResponse> Handle(GetBuildingByIdCommand request, CancellationToken cancellationToken)
    {
        var tenantId = tenantContext.TenantId ?? throw new InvalidOperationException("Tenant context is required");
        var building = await buildingRepository.GetByIdAsync(request.Id, tenantId, cancellationToken);

        if (building == null)
            return new GetBuildingByIdResponse(false, "Building not found", null);

        var dto = new BuildingDto(
            building.Id,
            building.SiteId,
            building.Name,
            building.AddressLine,
            building.ManagerContact,
            building.TenantId,
            building.CreatedAt,
            building.UpdatedAt
        );

        return new GetBuildingByIdResponse(true, null, dto);
    }
}

public class UpdateBuildingCommandHandler(IBuildingRepository buildingRepository, ITenantContext tenantContext)
    : IRequestHandler<UpdateBuildingCommand, UpdateBuildingResponse>
{
    public async Task<UpdateBuildingResponse> Handle(UpdateBuildingCommand request, CancellationToken cancellationToken)
    {
        var tenantId = tenantContext.TenantId ?? throw new InvalidOperationException("Tenant context is required");
        var existing = await buildingRepository.GetByIdAsync(request.Id, tenantId, cancellationToken);

        if (existing == null)
            return new UpdateBuildingResponse(false, "Building not found", null);

        existing.SiteId = request.SiteId;
        existing.Name = request.Name.Trim();
        existing.AddressLine = request.AddressLine;
        existing.ManagerContact = request.ManagerContact;
        existing.UpdatedBy = request.UpdatedBy;
        existing.UpdatedAt = DateTime.UtcNow;

        await buildingRepository.UpdateAsync(existing, cancellationToken);

        var dto = new BuildingDto(
            existing.Id,
            existing.SiteId,
            existing.Name,
            existing.AddressLine,
            existing.ManagerContact,
            existing.TenantId,
            existing.CreatedAt,
            existing.UpdatedAt
        );

        return new UpdateBuildingResponse(true, "Building updated successfully", dto);
    }
}

public class DeleteBuildingCommandHandler(IBuildingRepository buildingRepository, ITenantContext tenantContext, IPublisher publisher)
    : IRequestHandler<DeleteBuildingCommand, DeleteBuildingResponse>
{
    public async Task<DeleteBuildingResponse> Handle(DeleteBuildingCommand request, CancellationToken cancellationToken)
    {
        var tenantId = tenantContext.TenantId ?? throw new InvalidOperationException("Tenant context is required");
        var deleted = await buildingRepository.DeleteAsync(request.Id, tenantId, cancellationToken);

        if (deleted)
        {
            await publisher.Publish(new LocationDeletedNotification(request.Id, "Building"), cancellationToken);
        }

        return deleted 
            ? new DeleteBuildingResponse(true, "Building deleted successfully")
            : new DeleteBuildingResponse(false, "Building not found");
    }
}
