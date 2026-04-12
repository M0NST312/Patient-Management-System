using ClinicSystem.Application.Dtos;
using ClinicSystem.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicSystem.Web.Pages;

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
        var tasks = new Task[]
        {
            Task.Run(async () => PatientCount = await patientService.GetTotalPatientsCountAsync()),
            Task.Run(async () => VisitCount = await visitService.GetTotalVisitsCountAsync()),
            Task.Run(async () => InvoiceCount = await invoiceService.GetTotalInvoicesCountAsync()),
            Task.Run(async () => OutstandingBalance = await invoiceService.GetTotalOutstandingBalanceAsync()),
        };
        await Task.WhenAll(tasks);

        var allVisits = await visitService.GetAllVisitsAsync();
        RecentVisits = allVisits.OrderByDescending(v => v.VisitDate).Take(5).ToList();

        var unpaidInvoices = await invoiceService.GetInvoicesByStatusAsync(Domain.Enums.InvoiceStatus.Unpaid);
        var partialInvoices = await invoiceService.GetInvoicesByStatusAsync(Domain.Enums.InvoiceStatus.Partial);
        UnpaidInvoices = unpaidInvoices.Concat(partialInvoices)
            .OrderByDescending(i => i.Balance).Take(5).ToList();
    }
}
