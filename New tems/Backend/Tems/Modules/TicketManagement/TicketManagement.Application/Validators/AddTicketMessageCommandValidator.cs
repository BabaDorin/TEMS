using FluentValidation;
using TicketManagement.Contract.Commands.Tickets;

namespace TicketManagement.Application.Validators;

public class AddTicketMessageCommandValidator : AbstractValidator<AddTicketMessageCommand>
{
    public AddTicketMessageCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .NotEmpty().WithMessage("TicketId is required");

        RuleFor(x => x.SenderId)
            .NotEmpty().WithMessage("SenderId is required");

        RuleFor(x => x.SenderType)
            .NotEmpty().WithMessage("SenderType is required")
            .Must(BeValidSenderType).WithMessage("Invalid SenderType. Must be USER, AGENT, or AI_SYSTEM");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Message content is required")
            .MaximumLength(5000).WithMessage("Message content must not exceed 5000 characters");
    }

    private bool BeValidSenderType(string senderType)
    {
        var validTypes = new[] { "USER", "AGENT", "AI_SYSTEM" };
        return validTypes.Contains(senderType.ToUpper());
    }
}
