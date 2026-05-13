using MediatR;
using ChangeLog.Contract.Queries;
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

public record DeleteApprovalGateCommand(
    string TicketId,
    string ApprovalGateId
) : IRequest<UpdateApprovalGateResponse>;

public record GetTicketHistoryQuery(
    string TicketId,
    int PageNumber = 1,
    int PageSize = 50
) : IRequest<GetEntityTimelineResponse>;

public record GetTicketsForApprovalCommand() : IRequest<GetAllTicketsResponse>;
