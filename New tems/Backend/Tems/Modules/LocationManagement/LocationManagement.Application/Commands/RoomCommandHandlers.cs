using LocationManagement.Contract.DTOs;
using LocationManagement.Application.Domain;
using LocationManagement.Application.Interfaces;
using LocationManagement.Contract.Commands;
using LocationManagement.Contract.Responses;
using MediatR;
using Tems.Common.Notifications;
using Tems.Common.Tenant;

namespace LocationManagement.Application.Commands;

public class CreateRoomCommandHandler(IRoomRepository roomRepository, ITenantContext tenantContext, IPublisher publisher) 
    : IRequestHandler<CreateRoomCommand, CreateRoomResponse>
{
    public async Task<CreateRoomResponse> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        var tenantId = tenantContext.TenantId ?? throw new InvalidOperationException("Tenant context is required");

        var roomType = Enum.Parse<RoomType>(request.Type, true);
        var roomStatus = Enum.Parse<RoomStatus>(request.Status, true);

        var domainEntity = new Room
        {
            Id = Guid.NewGuid().ToString(),
            BuildingId = request.BuildingId,
            Name = request.Name.Trim(),
            RoomNumber = request.RoomNumber,
            FloorLabel = request.FloorLabel,
            Type = roomType,
            Capacity = request.Capacity,
            Area = request.Area,
            Status = roomStatus,
            Description = request.Description,
            TenantId = tenantId,
            CreatedBy = request.CreatedBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await roomRepository.CreateAsync(domainEntity, cancellationToken);

        await publisher.Publish(new LocationCreatedNotification(
            domainEntity.Id, domainEntity.Name, "Room", request.BuildingId, null, request.CreatedBy, null
        ), cancellationToken);

        var dto = new RoomDto(
            domainEntity.Id,
            domainEntity.BuildingId,
            domainEntity.Name,
            domainEntity.RoomNumber,
            domainEntity.FloorLabel,
            domainEntity.Type.ToString(),
            domainEntity.Capacity,
            domainEntity.Area,
            domainEntity.Status.ToString(),
            domainEntity.Description,
            domainEntity.TenantId,
            domainEntity.CreatedAt,
            domainEntity.UpdatedAt,
            null,
            null,
            null
        );

        return new CreateRoomResponse(true, "Room created successfully", dto);
    }
}

public class GetAllRoomsCommandHandler(
    IRoomRepository roomRepository, 
    IBuildingRepository buildingRepository,
    ISiteRepository siteRepository,
    ITenantContext tenantContext,
    IMediator mediator)
    : IRequestHandler<GetAllRoomsCommand, GetAllRoomsResponse>
{
    public async Task<GetAllRoomsResponse> Handle(GetAllRoomsCommand request, CancellationToken cancellationToken)
    {
        var tenantId = tenantContext.TenantId ?? throw new InvalidOperationException("Tenant context is required");
        var rooms = await roomRepository.GetAllAsync(tenantId, request.SiteId, request.BuildingId, cancellationToken);

        // Get all buildings and sites for enrichment
        var buildings = await buildingRepository.GetAllAsync(tenantId, request.SiteId, cancellationToken);
        var sites = await siteRepository.GetAllAsync(tenantId, cancellationToken);

        // Get asset counts for all rooms in a single call
        var roomIds = rooms.Select(r => r.Id).ToList();
        var assetCountsResponse = await mediator.Send(
            new AssetManagement.Contract.Commands.GetAssetCountsByLocationsCommand(roomIds),
            cancellationToken);
        var assetCounts = assetCountsResponse.Data;

        var roomDtos = rooms.Select(r =>
        {
            var building = buildings.FirstOrDefault(b => b.Id == r.BuildingId);
            var site = building != null ? sites.FirstOrDefault(s => s.Id == building.SiteId) : null;
            assetCounts.TryGetValue(r.Id, out var roomAssetCounts);

            return new RoomDto(
                r.Id,
                r.BuildingId,
                r.Name,
                r.RoomNumber,
                r.FloorLabel,
                r.Type.ToString(),
                r.Capacity,
                r.Area,
                r.Status.ToString(),
                r.Description,
                r.TenantId,
                r.CreatedAt,
                r.UpdatedAt,
                site?.Name,
                site?.Id,
                building?.Name,
                roomAssetCounts
            );
        }).ToList();

        return new GetAllRoomsResponse(true, null, roomDtos);
    }
}

public class GetRoomByIdCommandHandler(
    IRoomRepository roomRepository, 
    IBuildingRepository buildingRepository,
    ISiteRepository siteRepository,
    ITenantContext tenantContext,
    IMediator mediator)
    : IRequestHandler<GetRoomByIdCommand, GetRoomByIdResponse>
{
    public async Task<GetRoomByIdResponse> Handle(GetRoomByIdCommand request, CancellationToken cancellationToken)
    {
        var tenantId = tenantContext.TenantId ?? throw new InvalidOperationException("Tenant context is required");
        var room = await roomRepository.GetByIdAsync(request.Id, tenantId, cancellationToken);

        if (room == null)
            return new GetRoomByIdResponse(false, "Room not found", null);

        var building = await buildingRepository.GetByIdAsync(room.BuildingId, tenantId, cancellationToken);
        var site = building != null ? await siteRepository.GetByIdAsync(building.SiteId, tenantId, cancellationToken) : null;

        var assetCountsResponse = await mediator.Send(
            new AssetManagement.Contract.Commands.GetAssetCountsByLocationsCommand([request.Id]),
            cancellationToken);
        assetCountsResponse.Data.TryGetValue(request.Id, out var roomAssetCounts);

        var dto = new RoomDto(
            room.Id,
            room.BuildingId,
            room.Name,
            room.RoomNumber,
            room.FloorLabel,
            room.Type.ToString(),
            room.Capacity,
            room.Area,
            room.Status.ToString(),
            room.Description,
            room.TenantId,
            room.CreatedAt,
            room.UpdatedAt,
            site?.Name,
            site?.Id,
            building?.Name,
            roomAssetCounts
        );

        return new GetRoomByIdResponse(true, null, dto);
    }
}

public class UpdateRoomCommandHandler(IRoomRepository roomRepository, ITenantContext tenantContext)
    : IRequestHandler<UpdateRoomCommand, UpdateRoomResponse>
{
    public async Task<UpdateRoomResponse> Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
    {
        var tenantId = tenantContext.TenantId ?? throw new InvalidOperationException("Tenant context is required");
        var existing = await roomRepository.GetByIdAsync(request.Id, tenantId, cancellationToken);

        if (existing == null)
            return new UpdateRoomResponse(false, "Room not found", null);

        var roomType = Enum.Parse<RoomType>(request.Type, true);
        var roomStatus = Enum.Parse<RoomStatus>(request.Status, true);

        existing.BuildingId = request.BuildingId;
        existing.Name = request.Name.Trim();
        existing.RoomNumber = request.RoomNumber;
        existing.FloorLabel = request.FloorLabel;
        existing.Type = roomType;
        existing.Capacity = request.Capacity;
        existing.Area = request.Area;
        existing.Status = roomStatus;
        existing.Description = request.Description;
        existing.UpdatedBy = request.UpdatedBy;
        existing.UpdatedAt = DateTime.UtcNow;

        await roomRepository.UpdateAsync(existing, cancellationToken);

        var dto = new RoomDto(
            existing.Id,
            existing.BuildingId,
            existing.Name,
            existing.RoomNumber,
            existing.FloorLabel,
            existing.Type.ToString(),
            existing.Capacity,
            existing.Area,
            existing.Status.ToString(),
            existing.Description,
            existing.TenantId,
            existing.CreatedAt,
            existing.UpdatedAt,
            null,
            null,
            null
        );

        return new UpdateRoomResponse(true, "Room updated successfully", dto);
    }
}

public class DeleteRoomCommandHandler(IRoomRepository roomRepository, ITenantContext tenantContext, IPublisher publisher)
    : IRequestHandler<DeleteRoomCommand, DeleteRoomResponse>
{
    public async Task<DeleteRoomResponse> Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
    {
        var tenantId = tenantContext.TenantId ?? throw new InvalidOperationException("Tenant context is required");
        var deleted = await roomRepository.DeleteAsync(request.Id, tenantId, cancellationToken);

        if (deleted)
        {
            await publisher.Publish(new LocationDeletedNotification(request.Id, "Room"), cancellationToken);
        }

        return deleted 
            ? new DeleteRoomResponse(true, "Room deleted successfully")
            : new DeleteRoomResponse(false, "Room not found");
    }
}

public class GetLocationHierarchyCommandHandler(
    ISiteRepository siteRepository,
    IBuildingRepository buildingRepository,
    IRoomRepository roomRepository,
    ITenantContext tenantContext)
    : IRequestHandler<GetLocationHierarchyCommand, GetLocationHierarchyResponse>
{
    public async Task<GetLocationHierarchyResponse> Handle(GetLocationHierarchyCommand request, CancellationToken cancellationToken)
    {
        var tenantId = tenantContext.TenantId ?? throw new InvalidOperationException("Tenant context is required");
        
        var sites = await siteRepository.GetAllAsync(tenantId, cancellationToken);
        var buildings = await buildingRepository.GetAllAsync(tenantId, null, cancellationToken);
        var rooms = await roomRepository.GetAllAsync(tenantId, null, null, cancellationToken);

        var hierarchy = new List<LocationHierarchyDto>();

        foreach (var site in sites)
        {
            hierarchy.Add(new LocationHierarchyDto(
                site.Id,
                site.Name,
                "Site",
                null,
                site.Code,
                null,
                site.IsActive ? "Active" : "Inactive"
            ));
        }

        foreach (var building in buildings)
        {
            hierarchy.Add(new LocationHierarchyDto(
                building.Id,
                building.Name,
                "Building",
                building.SiteId,
                null,
                null,
                null
            ));
        }

        foreach (var room in rooms)
        {
            hierarchy.Add(new LocationHierarchyDto(
                room.Id,
                room.Name,
                "Room",
                room.BuildingId,
                null,
                room.FloorLabel,
                room.Status.ToString()
            ));
        }

        return new GetLocationHierarchyResponse(true, null, hierarchy);
    }
}
