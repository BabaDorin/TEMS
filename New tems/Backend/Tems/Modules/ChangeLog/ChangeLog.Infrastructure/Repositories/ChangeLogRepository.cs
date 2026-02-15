using ChangeLog.Application.Domain;
using ChangeLog.Application.Domain.AssetLogs;
using ChangeLog.Application.Domain.LocationLogs;
using ChangeLog.Application.Domain.UserLogs;
using ChangeLog.Application.Interfaces;
using ChangeLog.Contract.Enums;
using ChangeLog.Infrastructure.Entities;
using ChangeLog.Infrastructure.Mappers;
using MongoDB.Driver;

namespace ChangeLog.Infrastructure.Repositories;

public class ChangeLogRepository(IMongoDatabase database) : IChangeLogRepository
{
    private readonly IMongoCollection<AssetChangeLogEntity> _assetLogs =
        database.GetCollection<AssetChangeLogEntity>("asset_change_logs");

    private readonly IMongoCollection<UserChangeLogEntity> _userLogs =
        database.GetCollection<UserChangeLogEntity>("user_change_logs");

    private readonly IMongoCollection<LocationChangeLogEntity> _locationLogs =
        database.GetCollection<LocationChangeLogEntity>("location_change_logs");

    public async Task CreateAsync(ChangeLogEntry entry, CancellationToken cancellationToken = default)
    {
        switch (entry.EntityType)
        {
            case ChangeLogEntityType.Asset:
                await _assetLogs.InsertOneAsync(AssetChangeLogMapper.ToDatabase(entry), cancellationToken: cancellationToken);
                break;
            case ChangeLogEntityType.User:
                await _userLogs.InsertOneAsync(UserChangeLogMapper.ToDatabase(entry), cancellationToken: cancellationToken);
                break;
            case ChangeLogEntityType.Location:
                await _locationLogs.InsertOneAsync(LocationChangeLogMapper.ToDatabase(entry), cancellationToken: cancellationToken);
                break;
            default:
                throw new InvalidOperationException($"Unknown entity type: {entry.EntityType}");
        }
    }

    public async Task<(List<ChangeLogEntry> Entries, int TotalCount)> GetByEntityAsync(
        ChangeLogEntityType entityType,
        string entityId,
        string tenantId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return entityType switch
        {
            ChangeLogEntityType.Asset => await GetAssetLogsAsync(entityId, tenantId, pageNumber, pageSize, cancellationToken),
            ChangeLogEntityType.User => await GetUserLogsAsync(entityId, tenantId, pageNumber, pageSize, cancellationToken),
            ChangeLogEntityType.Location => await GetLocationLogsAsync(entityId, tenantId, pageNumber, pageSize, cancellationToken),
            _ => throw new InvalidOperationException($"Unknown entity type: {entityType}")
        };
    }

    private async Task<(List<ChangeLogEntry> Entries, int TotalCount)> GetAssetLogsAsync(
        string entityId, string tenantId, int pageNumber, int pageSize, CancellationToken ct)
    {
        var filter = Builders<AssetChangeLogEntity>.Filter.And(
            Builders<AssetChangeLogEntity>.Filter.Eq(x => x.AssetId, entityId),
            Builders<AssetChangeLogEntity>.Filter.Eq(x => x.TenantId, tenantId)
        );

        var totalCount = await _assetLogs.CountDocumentsAsync(filter, cancellationToken: ct);
        var entities = await _assetLogs
            .Find(filter)
            .SortByDescending(x => x.Timestamp)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(ct);

        return (entities.Select(e => e.ToDomain()).ToList(), (int)totalCount);
    }

    private async Task<(List<ChangeLogEntry> Entries, int TotalCount)> GetUserLogsAsync(
        string entityId, string tenantId, int pageNumber, int pageSize, CancellationToken ct)
    {
        var filter = Builders<UserChangeLogEntity>.Filter.And(
            Builders<UserChangeLogEntity>.Filter.Eq(x => x.TargetUserId, entityId),
            Builders<UserChangeLogEntity>.Filter.Eq(x => x.TenantId, tenantId)
        );

        var totalCount = await _userLogs.CountDocumentsAsync(filter, cancellationToken: ct);
        var entities = await _userLogs
            .Find(filter)
            .SortByDescending(x => x.Timestamp)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(ct);

        return (entities.Select(e => e.ToDomain()).ToList(), (int)totalCount);
    }

    private async Task<(List<ChangeLogEntry> Entries, int TotalCount)> GetLocationLogsAsync(
        string entityId, string tenantId, int pageNumber, int pageSize, CancellationToken ct)
    {
        var filter = Builders<LocationChangeLogEntity>.Filter.And(
            Builders<LocationChangeLogEntity>.Filter.Eq(x => x.LocationId, entityId),
            Builders<LocationChangeLogEntity>.Filter.Eq(x => x.TenantId, tenantId)
        );

        var totalCount = await _locationLogs.CountDocumentsAsync(filter, cancellationToken: ct);
        var entities = await _locationLogs
            .Find(filter)
            .SortByDescending(x => x.Timestamp)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(ct);

        return (entities.Select(e => e.ToDomain()).ToList(), (int)totalCount);
    }
}
