using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Contract.Commands;

public record GetAllAssetCommand(bool IncludeArchived = false) : IRequest<GetAllAssetResponse>;
