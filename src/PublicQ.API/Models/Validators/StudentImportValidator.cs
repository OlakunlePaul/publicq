using FluentValidation;
using PublicQ.Application.Models;

namespace PublicQ.API.Models.Validators;

/// <summary>
/// Student import validation
/// </summary>
public class StudentImportValidator : AbstractValidator<IList<StudentImportDto>>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public StudentImportValidator()
    {
        RuleFor(x => x)
            .NotNull()
            .WithMessage("Students list cannot be null.")
            .NotEmpty()
            .WithMessage("At least one student must be provided.");

        RuleForEach(x => x).ChildRules(student =>
        {
            student.RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Student name is required.")
                .MaximumLength(200)
                .WithMessage("Student name must not exceed 200 characters.");

            student.RuleFor(x => x.Email)
                .EmailAddress()
                .When(x => !string.IsNullOrWhiteSpace(x.Email))
                .WithMessage("Email must be a valid email address when provided.")
                .MaximumLength(254)
                .When(x => !string.IsNullOrWhiteSpace(x.Email))
                .WithMessage("Email must not exceed 254 characters when provided.");
        });
    }
}
