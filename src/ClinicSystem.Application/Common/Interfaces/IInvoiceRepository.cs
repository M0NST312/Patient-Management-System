using ClinicSystem.Domain.Entities;
using ClinicSystem.Domain.Enums;

namespace ClinicSystem.Application.Common.Interfaces;

public interface IInvoiceRepository : IRepository<Invoice>
{
    Task<IEnumerable<Invoice>> GetAllWithDetailsAsync(CancellationToken ct = default);
    Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber, CancellationToken ct = default);
    Task<IEnumerable<Invoice>> GetByPatientIdAsync(Guid patientId, CancellationToken ct = default);
    Task<IEnumerable<Invoice>> GetByStatusAsync(InvoiceStatus status, CancellationToken ct = default);
    Task<decimal> GetOutstandingBalanceAsync(Guid patientId, CancellationToken ct = default);
    Task<string> GenerateNextInvoiceNumberAsync(CancellationToken ct = default);
    Task<Invoice?> GetWithItemsAndPaymentsAsync(Guid invoiceId, CancellationToken ct = default);
}
