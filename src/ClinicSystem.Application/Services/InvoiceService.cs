using ClinicSystem.Application.Common.Interfaces;
using ClinicSystem.Application.Dtos;
using ClinicSystem.Application.Services.Interfaces;
using ClinicSystem.Domain.Entities;
using ClinicSystem.Domain.Enums;

namespace ClinicSystem.Application.Services;

public class InvoiceService(IInvoiceRepository repository, IPatientRepository patientRepository) : IInvoiceService
{
    public async Task<InvoiceDetailsDto?> GetInvoiceByIdAsync(Guid id, CancellationToken ct = default)
    {
        var invoice = await repository.GetWithItemsAndPaymentsAsync(id, ct);
        return invoice is null ? null : MapToDetails(invoice);
    }

    public async Task<IEnumerable<InvoiceDetailsDto>> GetAllInvoicesAsync(CancellationToken ct = default)
    {
        var invoices = await repository.GetAllWithDetailsAsync(ct);
        return invoices.Select(MapToDetails).ToList();
    }

    public async Task<IEnumerable<InvoiceDetailsDto>> GetInvoicesByPatientIdAsync(Guid patientId, CancellationToken ct = default)
    {
        var invoices = await repository.GetByPatientIdAsync(patientId, ct);
        return invoices.Select(MapToDetails).ToList();
    }

    public async Task<IEnumerable<InvoiceDetailsDto>> GetInvoicesByStatusAsync(InvoiceStatus status, CancellationToken ct = default)
    {
        var invoices = await repository.GetByStatusAsync(status, ct);
        return invoices.Select(MapToDetails).ToList();
    }

    public async Task<Guid> CreateInvoiceAsync(InvoiceCreateDto dto, CancellationToken ct = default)
    {
        _ = await patientRepository.GetByIdAsync(dto.PatientId, ct)
            ?? throw new KeyNotFoundException($"Patient with ID '{dto.PatientId}' not found.");

        if (dto.Items is null or [] )
            throw new ArgumentException("Invoice must have at least one item.");

        // Validate invoice items
        foreach (var item in dto.Items)
        {
            if (item.UnitPrice <= 0)
                throw new ArgumentException("Unit price must be greater than zero.");
            if (item.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");
        }

        // Validate discount
        if (dto.DiscountAmount < 0)
            throw new ArgumentException("Discount amount cannot be negative.");
        
        var totalAmount = dto.Items.Sum(i => i.UnitPrice * i.Quantity);
        if (dto.DiscountAmount > totalAmount)
            throw new ArgumentException($"Discount amount ({dto.DiscountAmount:C}) cannot exceed total amount ({totalAmount:C}).");

        var invoiceNumber = await repository.GenerateNextInvoiceNumberAsync(ct);
        var invoice = new Invoice
        {
            PatientId = dto.PatientId,
            InvoiceNumber = invoiceNumber,
            DiscountAmount = dto.DiscountAmount,
            Status = InvoiceStatus.Unpaid
        };

        foreach (var item in dto.Items)
            invoice.Items.Add(new InvoiceItem { Description = item.Description, UnitPrice = item.UnitPrice, Quantity = item.Quantity });

        await repository.AddAsync(invoice, ct);
        await repository.SaveChangesAsync(ct);
        return invoice.Id;
    }

    public async Task UpdateInvoiceAsync(Guid id, InvoiceUpdateDto dto, CancellationToken ct = default)
    {
        var invoice = await repository.GetWithItemsAndPaymentsAsync(id, ct)
            ?? throw new KeyNotFoundException($"Invoice with ID '{id}' not found.");

        if (invoice.Status == InvoiceStatus.Paid)
            throw new InvalidOperationException("Cannot update a paid invoice.");

        invoice.DiscountAmount = dto.DiscountAmount;
        invoice.Items.Clear();
        foreach (var item in dto.Items)
            invoice.Items.Add(new InvoiceItem { Description = item.Description, UnitPrice = item.UnitPrice, Quantity = item.Quantity });

        repository.Update(invoice);
        await repository.SaveChangesAsync(ct);
    }

    public async Task DeleteInvoiceAsync(Guid id, CancellationToken ct = default)
    {
        var invoice = await repository.GetWithItemsAndPaymentsAsync(id, ct)
            ?? throw new KeyNotFoundException($"Invoice with ID '{id}' not found.");

        if (invoice.Status != InvoiceStatus.Unpaid)
            throw new InvalidOperationException("Cannot delete a paid or partially paid invoice.");

        repository.Delete(invoice);
        await repository.SaveChangesAsync(ct);
    }

    public async Task AddPaymentAsync(Guid invoiceId, PaymentCreateDto dto, CancellationToken ct = default)
    {
        var invoice = await repository.GetWithItemsAndPaymentsAsync(invoiceId, ct)
            ?? throw new KeyNotFoundException($"Invoice with ID '{invoiceId}' not found.");

        if (invoice.Status == InvoiceStatus.Paid)
            throw new InvalidOperationException("Invoice is already fully paid.");

        if (dto.Amount <= 0)
            throw new ArgumentException("Payment amount must be greater than zero.");

        var totalAmount = invoice.Items.Sum(x => x.Total) - invoice.DiscountAmount;
        var totalPaid = invoice.Payments.Sum(p => p.Amount) + dto.Amount;

        if (totalPaid > totalAmount)
            throw new ArgumentException($"Payment of {dto.Amount:C} would exceed invoice balance of {invoice.Balance:C}.");

        invoice.Payments.Add(new Payment { InvoiceId = invoiceId, Amount = dto.Amount, Method = dto.Method, PaidAtUtc = DateTime.UtcNow });
        invoice.Status = totalPaid >= totalAmount ? InvoiceStatus.Paid : InvoiceStatus.Partial;

        repository.Update(invoice);
        await repository.SaveChangesAsync(ct);
    }

    public async Task<decimal> GetTotalOutstandingBalanceAsync(CancellationToken ct = default)
    {
        var unpaid = await repository.GetByStatusAsync(InvoiceStatus.Unpaid, ct);
        var partial = await repository.GetByStatusAsync(InvoiceStatus.Partial, ct);
        return unpaid.Concat(partial).Sum(i => i.Balance);
    }

    public async Task<int> GetTotalInvoicesCountAsync(CancellationToken ct = default) =>
        await repository.CountAsync(null, ct);

    private static InvoiceDetailsDto MapToDetails(Invoice invoice)
    {
        var total = invoice.Items.Sum(x => x.Total) - invoice.DiscountAmount;
        var paid = invoice.Payments.Sum(p => p.Amount);
        return new InvoiceDetailsDto(
            invoice.Id, invoice.PatientId, invoice.InvoiceNumber, invoice.DiscountAmount, invoice.Status,
            invoice.Items.Select(i => new InvoiceItemDetailsDto(i.Id, i.Description, i.UnitPrice, i.Quantity, i.Total)).ToList(),
            invoice.Payments.Select(p => new PaymentDto(p.Id, p.Amount, p.Method, p.PaidAtUtc)).ToList(),
            invoice.CreatedAtUtc, invoice.Patient?.FullName, total, paid, total - paid
        );
    }
}
