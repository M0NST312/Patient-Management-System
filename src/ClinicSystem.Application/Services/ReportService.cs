using ClinicSystem.Application.Dtos;
using ClinicSystem.Application.Services.Interfaces;
using ClinicSystem.Application.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace ClinicSystem.Application.Services;

public class ReportService(IInvoiceService invoiceService, IPatientService patientService, IVisitService visitService, ILogger<ReportService> logger) : IReportService
{
    public async Task<IEnumerable<InvoiceDetailsDto>> GetInvoicesForReportAsync(DateTime? from = null, DateTime? to = null, CancellationToken ct = default)
    {
        var invoices = (await invoiceService.GetAllInvoicesAsync(ct)).ToList();
        if (from.HasValue)
            invoices = invoices.Where(i => i.CreatedAtUtc >= from.Value).ToList();
        if (to.HasValue)
            invoices = invoices.Where(i => i.CreatedAtUtc <= to.Value).ToList();
        return invoices;
    }

    public async Task<IEnumerable<PatientDetailsDto>> GetPatientsForReportAsync(CancellationToken ct = default)
    {
        return await patientService.GetAllPatientsAsync(ct);
    }

    public async Task<IEnumerable<VisitDetailsDto>> GetVisitsForReportAsync(DateTime? from = null, DateTime? to = null, CancellationToken ct = default)
    {
        var visits = (await visitService.GetAllVisitsAsync(ct)).ToList();
        if (from.HasValue)
            visits = visits.Where(v => v.VisitDate >= from.Value).ToList();
        if (to.HasValue)
            visits = visits.Where(v => v.VisitDate <= to.Value).ToList();
        return visits;
    }
}
