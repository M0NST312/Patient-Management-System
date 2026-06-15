using ClinicSystem.Application.Dtos;

namespace ClinicSystem.Application.Services.Interfaces;

public interface IReportService
{
    Task<IEnumerable<InvoiceDetailsDto>> GetInvoicesForReportAsync(DateTime? from = null, DateTime? to = null, CancellationToken ct = default);
    Task<IEnumerable<PatientDetailsDto>> GetPatientsForReportAsync(CancellationToken ct = default);
    Task<IEnumerable<VisitDetailsDto>> GetVisitsForReportAsync(DateTime? from = null, DateTime? to = null, CancellationToken ct = default);
}
