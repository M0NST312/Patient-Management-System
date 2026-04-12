using ClinicSystem.Application.Common.Interfaces;
using ClinicSystem.Domain.Entities;
using ClinicSystem.Domain.Enums;
using ClinicSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicSystem.Infrastructure.Repositories;

public class InvoiceRepository(ApplicationDbContext context) : Repository<Invoice>(context), IInvoiceRepository
{
    public async Task<IEnumerable<Invoice>> GetAllWithDetailsAsync(CancellationToken ct = default) =>
        await DbSet.Include(i => i.Patient).Include(i => i.Items).Include(i => i.Payments)
            .OrderByDescending(i => i.CreatedAtUtc).ToListAsync(ct);

    public async Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber, CancellationToken ct = default) =>
        await DbSet.Where(i => i.InvoiceNumber == invoiceNumber)
            .Include(i => i.Patient).Include(i => i.Items).Include(i => i.Payments)
            .FirstOrDefaultAsync(ct);

    public async Task<IEnumerable<Invoice>> GetByPatientIdAsync(Guid patientId, CancellationToken ct = default) =>
        await DbSet.Where(i => i.PatientId == patientId)
            .Include(i => i.Items).Include(i => i.Payments)
            .OrderByDescending(i => i.CreatedAtUtc).ToListAsync(ct);

    public async Task<IEnumerable<Invoice>> GetByStatusAsync(InvoiceStatus status, CancellationToken ct = default) =>
        await DbSet.Where(i => i.Status == status)
            .Include(i => i.Patient).Include(i => i.Items).Include(i => i.Payments)
            .OrderByDescending(i => i.CreatedAtUtc).ToListAsync(ct);

    public async Task<decimal> GetOutstandingBalanceAsync(Guid patientId, CancellationToken ct = default)
    {
        var invoices = await DbSet
            .Where(i => i.PatientId == patientId && i.Status != InvoiceStatus.Paid)
            .Include(i => i.Items).Include(i => i.Payments).ToListAsync(ct);
        return invoices.Sum(i => i.Balance);
    }

    public async Task<Invoice?> GetWithItemsAndPaymentsAsync(Guid invoiceId, CancellationToken ct = default) =>
        await DbSet.Where(i => i.Id == invoiceId)
            .Include(i => i.Patient).Include(i => i.Items).Include(i => i.Payments)
            .FirstOrDefaultAsync(ct);

    public async Task<string> GenerateNextInvoiceNumberAsync(CancellationToken ct = default)
    {
        var year = DateTime.UtcNow.Year;
        var count = await DbSet.CountAsync(i => i.CreatedAtUtc.Year == year, ct);
        return $"INV-{year}-{(count + 1):D4}";
    }
}
