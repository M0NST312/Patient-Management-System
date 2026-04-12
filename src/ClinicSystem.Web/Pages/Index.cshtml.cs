using ClinicSystem.Application.Dtos;
using ClinicSystem.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicSystem.Web.Pages;

[Authorize]
public class IndexModel(IPatientService patientService, IVisitService visitService, IInvoiceService invoiceService) : PageModel
{
    public int PatientCount { get; private set; }
    public int VisitCount { get; private set; }
    public int InvoiceCount { get; private set; }
    public decimal OutstandingBalance { get; private set; }
    public List<VisitDetailsDto> RecentVisits { get; private set; } = [];
    public List<InvoiceDetailsDto> UnpaidInvoices { get; private set; } = [];

    public async Task OnGetAsync()
    {
        PatientCount = await patientService.GetTotalPatientsCountAsync();
        VisitCount = await visitService.GetTotalVisitsCountAsync();
        InvoiceCount = await invoiceService.GetTotalInvoicesCountAsync();
        OutstandingBalance = await invoiceService.GetTotalOutstandingBalanceAsync();

        RecentVisits = (await visitService.GetRecentVisitsAsync(5)).ToList();

        var unpaidInvoices = await invoiceService.GetInvoicesByStatusAsync(Domain.Enums.InvoiceStatus.Unpaid);
        var partialInvoices = await invoiceService.GetInvoicesByStatusAsync(Domain.Enums.InvoiceStatus.Partial);
        UnpaidInvoices = unpaidInvoices.Concat(partialInvoices)
            .OrderByDescending(i => i.Balance).Take(5).ToList();
    }
}
