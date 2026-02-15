using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Contract.Commands;

public record GetAssetCountsByUsersCommand(List<string> UserIds) : IRequest<GetAssetCountsByUsersResponse>;
