using FluentValidation;
using PublicQ.API.Models.Requests;

namespace PublicQ.API.Models.Validators;

public class StudentEnrollmentRequestValidator : AbstractValidator<StudentEnrollmentRequest>
{
    public StudentEnrollmentRequestValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty().WithMessage("Session ID is required.");
        RuleFor(x => x.TermId).NotEmpty().WithMessage("Term ID is required.");
    }
}
