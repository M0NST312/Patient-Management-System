using ClinicSystem.Application.Dtos;
using FluentValidation;

namespace ClinicSystem.Application.Validators;

public class PaymentCreateValidator : AbstractValidator<PaymentCreateDto>
{
    public PaymentCreateValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Payment amount must be greater than zero.")
            .LessThanOrEqualTo(decimal.MaxValue / 100)
            .WithMessage("Payment amount is too large.");

        RuleFor(x => x.Method)
            .NotEmpty().WithMessage("Payment method is required.")
            .MaximumLength(30).WithMessage("Payment method cannot exceed 30 characters.")
            .Matches(@"^[a-zA-Z\s]+$").WithMessage("Payment method can only contain letters and spaces.");
    }
}
