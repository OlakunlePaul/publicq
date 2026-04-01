using FluentValidation;
using PublicQ.Application.Models.Group;

namespace PublicQ.API.Models.Validators;

/// <summary>
/// Validator for <see cref="GroupUpdateDto"/>
/// </summary>
public class GroupUpdateValidator : AbstractValidator<GroupUpdateDto>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public GroupUpdateValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Group ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Group title is required.")
            .MaximumLength(200)
            .WithMessage("Group title must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(5000)
            .When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Group description must not exceed 5000 characters.");
    }
}
