using TicketManagement.Application.Domain;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Application.Helpers;

public static class ApprovalGateResponseMapper
{
    public static ApprovalGateResponse ToResponse(this ApprovalGate domainEntity)
    {
        return new ApprovalGateResponse(
            domainEntity.ApprovalGateId,
            domainEntity.Title,
            domainEntity.Justification,
            domainEntity.State,
            domainEntity.AllApproversRequired,
            domainEntity.Approvers.Select(x => x.ToResponse()).ToList(),
            domainEntity.CreatedAt,
            domainEntity.UpdatedAt
        );
    }

    public static ApprovalGateApproverResponse ToResponse(this ApprovalGateApprover domainEntity)
    {
        return new ApprovalGateApproverResponse(
            domainEntity.UserId,
            domainEntity.Status,
            domainEntity.ReviewedAt
        );
    }
}
