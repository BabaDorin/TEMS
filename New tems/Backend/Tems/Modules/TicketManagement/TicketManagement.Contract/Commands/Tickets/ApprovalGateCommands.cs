using MediatR;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Contract.Commands.Tickets;

public record CreateApprovalGateCommand(
    string TicketId,
    string Title,
    string Justification,
    bool AllApproversRequired,
    List<string> ApproverUserIds
) : IRequest<CreateApprovalGateResponse>;

public record UpdateApprovalGateCommand(
    string TicketId,
    string ApprovalGateId,
    string Title,
    string Justification,
    bool AllApproversRequired,
    List<string> ApproverUserIds
) : IRequest<UpdateApprovalGateResponse>;

public record ReviewApprovalGateCommand(
    string TicketId,
    string ApprovalGateId,
    string Status
) : IRequest<ReviewApprovalGateResponse>;

public record GetTicketsForApprovalCommand() : IRequest<GetAllTicketsResponse>;
