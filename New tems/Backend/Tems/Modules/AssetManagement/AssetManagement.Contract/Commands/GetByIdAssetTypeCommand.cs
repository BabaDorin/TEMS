using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Contract.Commands;

public record GetByIdAssetTypeCommand(string Id) : IRequest<GetByIdAssetTypeResponse>;
