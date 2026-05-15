using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Contract.Commands;

public record DeletePurchaseOrderCommand(string Id) : IRequest<DeletePurchaseOrderResponse>;
