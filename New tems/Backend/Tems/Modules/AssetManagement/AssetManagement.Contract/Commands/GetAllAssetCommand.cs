using AssetManagement.Contract.DTOs;
using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Contract.Commands;

public record GetAllAssetCommand(
    AssetFilterDto? Filter = null,
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<GetAllAssetResponse>;
