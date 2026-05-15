using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Contract.Commands;

public record GetPurchaseOrderByIdCommand(string Id) : IRequest<GetPurchaseOrderByIdResponse>;
