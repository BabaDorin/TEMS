using FluentValidation;
using TicketManagement.Contract.Commands.TicketTypes;

namespace TicketManagement.Application.Validators;

public class UpdateTicketTypeCommandValidator : AbstractValidator<UpdateTicketTypeCommand>
{
    public UpdateTicketTypeCommandValidator()
    {
        RuleFor(x => x.TicketTypeId)
            .NotEmpty().WithMessage("TicketTypeId is required");

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

        When(x => x.WorkflowConfig != null, () =>
        {
            RuleFor(x => x.WorkflowConfig!.States)
                .NotEmpty().WithMessage("At least one workflow state is required");

            RuleFor(x => x.WorkflowConfig!.InitialStateId)
                .NotEmpty().WithMessage("Initial state ID is required");
        });

        RuleFor(x => x.AttributeDefinitions)
            .Must(attributes => attributes == null || attributes.Count <= 50)
            .WithMessage("Maximum 50 attributes allowed per ticket type");

        When(x => x.AttributeDefinitions != null && x.AttributeDefinitions.Any(), () =>
        {
            RuleForEach(x => x.AttributeDefinitions).ChildRules(attribute =>
            {
                attribute.RuleFor(a => a.Key)
                    .NotEmpty().WithMessage("Attribute key is required");

                attribute.RuleFor(a => a.Label)
                    .NotEmpty().WithMessage("Attribute label is required");

                attribute.RuleFor(a => a.DataType)
                    .NotEmpty().WithMessage("Attribute data type is required");

                attribute.RuleFor(a => a.Options)
                    .Must((attr, options) => attr.DataType.ToUpper() != "DROPDOWN" || (options != null && options.Any()))
                    .WithMessage("Dropdown attributes must have at least one option");
            });
        });
    }

    private bool BeValidItilCategory(string category)
    {
        var validCategories = new[] { "INCIDENT", "PROBLEM", "CHANGE", "REQUEST", "SECURITY_INCIDENT", "ALERT" };
        return validCategories.Contains(category.ToUpper());
    }
}
