using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Contract.Commands;

public record GetAllAssetTypeCommand(bool IncludeArchived = false) : IRequest<GetAllAssetTypeResponse>;
