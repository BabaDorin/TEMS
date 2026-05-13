using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Contract.Commands;

public record AddAssetHistoryLogCommand(
    string Id,
    string Type,
    string Description,
    bool CostIncluded,
    decimal? CostAmount,
    string? CostCurrency) : IRequest<AddAssetHistoryLogResponse>;
