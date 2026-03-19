using FluentValidation;
using PublicQ.Infrastructure.Options;

namespace PublicQ.API.Models.Validators;

/// <summary>
/// Validator for Resend options.
/// </summary>
public class ResendOptionsValidator : AbstractValidator<ResendOptions>
{
    public ResendOptionsValidator()
    {
        RuleFor(x => x.ApiKey)
            .NotEmpty()
            .WithMessage("Resend API Key is required.");
    }
}
