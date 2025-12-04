using FluentValidation;
using TicketManagement.Contract.Commands.TicketTypes;

namespace TicketManagement.Application.Validators;

public class CreateTicketTypeCommandValidator : AbstractValidator<CreateTicketTypeCommand>
{
    public CreateTicketTypeCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("TenantId is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");

        RuleFor(x => x.ItilCategory)
            .NotEmpty().WithMessage("ITIL Category is required")
            .Must(BeValidItilCategory).WithMessage("Invalid ITIL Category");

        RuleFor(x => x.Version)
            .GreaterThan(0).WithMessage("Version must be greater than 0");

        RuleFor(x => x.WorkflowConfig)
            .NotNull().WithMessage("Workflow configuration is required");

        RuleFor(x => x.WorkflowConfig.States)
            .NotEmpty().WithMessage("At least one workflow state is required");

        RuleFor(x => x.WorkflowConfig.InitialStateId)
            .NotEmpty().WithMessage("Initial state ID is required");

        RuleFor(x => x.AttributeDefinitions)
            .NotNull().WithMessage("Attribute definitions are required");
    }

    private bool BeValidItilCategory(string category)
    {
        var validCategories = new[] { "INCIDENT", "PROBLEM", "CHANGE", "REQUEST", "SECURITY_INCIDENT", "ALERT" };
        return validCategories.Contains(category.ToUpper());
    }
}
