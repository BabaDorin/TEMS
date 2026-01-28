using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Contract.Commands;

public record GetAssetCountsByLocationsCommand(List<string> LocationIds) : IRequest<GetAssetCountsByLocationsResponse>;
