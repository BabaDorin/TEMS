using FluentValidation;
using TicketManagement.Contract.Commands.Tickets;

namespace TicketManagement.Application.Validators;

public class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
{
    public CreateTicketCommandValidator()
    {
        RuleFor(x => x.TicketTypeId)
            .NotEmpty().WithMessage("TicketTypeId is required");

        RuleFor(x => x.Summary)
            .NotEmpty().WithMessage("Summary is required")
            .MaximumLength(500).WithMessage("Summary must not exceed 500 characters");

        RuleFor(x => x.Priority)
            .NotEmpty().WithMessage("Priority is required")
            .Must(BeValidPriority).WithMessage("Invalid Priority. Must be LOW, MEDIUM, HIGH, or CRITICAL");

        RuleFor(x => x.Reporter)
            .NotNull().WithMessage("Reporter information is required");

        RuleFor(x => x.Reporter.UserId)
            .NotEmpty().When(x => x.Reporter != null)
            .WithMessage("Reporter UserId is required");

        RuleFor(x => x.Reporter.ChannelSource)
            .NotEmpty().When(x => x.Reporter != null)
            .WithMessage("Reporter ChannelSource is required")
            .Must(BeValidChannelSource).When(x => x.Reporter != null)
            .WithMessage("Invalid ChannelSource. Must be TEAMS, SLACK, or WEB");
    }

    private bool BeValidPriority(string priority)
    {
        var validPriorities = new[] { "LOW", "MEDIUM", "HIGH", "CRITICAL" };
        return validPriorities.Contains(priority.ToUpper());
    }

    private bool BeValidChannelSource(string channelSource)
    {
        var validSources = new[] { "TEAMS", "SLACK", "WEB" };
        return validSources.Contains(channelSource.ToUpper());
    }
}
