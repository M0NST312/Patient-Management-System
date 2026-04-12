using ClinicSystem.Application.Dtos;
using FluentValidation;

namespace ClinicSystem.Application.Validators;

public class InvoiceCreateValidator : AbstractValidator<InvoiceCreateDto>
{
    public InvoiceCreateValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("Patient ID is required.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Invoice must have at least one item.")
            .Must(items => items != null && items.Any()).WithMessage("Items list cannot be empty.");

        RuleForEach(x => x.Items).SetValidator(new InvoiceItemValidator());

        RuleFor(x => x.DiscountAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Discount amount cannot be negative.")
            .Must((dto, discount) =>
            {
                if (dto.Items == null || !dto.Items.Any()) return true;
                var total = dto.Items.Sum(i => i.UnitPrice * i.Quantity);
                return discount <= total;
            })
            .WithMessage("Discount amount cannot exceed the total invoice amount.");
    }
}

public class InvoiceItemValidator : AbstractValidator<InvoiceItemDto>
{
    public InvoiceItemValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Item description is required.")
            .MaximumLength(500).WithMessage("Item description cannot exceed 500 characters.");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0).WithMessage("Unit price must be greater than zero.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.")
            .LessThanOrEqualTo(9999).WithMessage("Quantity cannot exceed 9999.");
    }
}

public class InvoiceUpdateValidator : AbstractValidator<InvoiceUpdateDto>
{
    public InvoiceUpdateValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Invoice must have at least one item.")
            .Must(items => items != null && items.Any()).WithMessage("Items list cannot be empty.");

        RuleForEach(x => x.Items).SetValidator(new InvoiceItemValidator());

        RuleFor(x => x.DiscountAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Discount amount cannot be negative.")
            .Must((dto, discount) =>
            {
                if (dto.Items == null || !dto.Items.Any()) return true;
                var total = dto.Items.Sum(i => i.UnitPrice * i.Quantity);
                return discount <= total;
            })
            .WithMessage("Discount amount cannot exceed the total invoice amount.");
    }
}
