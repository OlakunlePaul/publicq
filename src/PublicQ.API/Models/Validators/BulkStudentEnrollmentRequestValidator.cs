using FluentValidation;
using PublicQ.API.Models.Requests;

namespace PublicQ.API.Models.Validators;

public class BulkStudentEnrollmentRequestValidator : AbstractValidator<BulkStudentEnrollmentRequest>
{
    public BulkStudentEnrollmentRequestValidator()
    {
        RuleFor(x => x.StudentIds).NotEmpty().WithMessage("At least one student must be selected.");
        RuleFor(x => x.SessionId).NotEmpty().WithMessage("Session ID is required.");
        RuleFor(x => x.TermId).NotEmpty().WithMessage("Term ID is required.");
        RuleFor(x => x.ClassLevelId).NotEmpty().WithMessage("Class Level ID is required.");
    }
}
