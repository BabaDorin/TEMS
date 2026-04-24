using FluentValidation;
using TicketManagement.Contract.Commands.Tickets;

namespace TicketManagement.Application.Validators;

public class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
{
    public CreateTicketCommandValidator()
    {
        RuleFor(x => x.TicketTypeId)
            .NotEmpty().WithMessage("TicketTypeId is required");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Issue title is required")
            .MaximumLength(50).WithMessage("Issue title must not exceed 50 characters");

        RuleFor(x => x.Summary)
            .NotEmpty().WithMessage("Problem description is required")
            .MaximumLength(2000).WithMessage("Problem description must not exceed 2000 characters");

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
