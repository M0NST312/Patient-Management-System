using ClinicSystem.Application.Dtos;
using ClinicSystem.Application.Services.Interfaces;
using ClinicSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicSystem.Web.Pages.Billing;

[Authorize(Roles = "Admin,Receptionist")]
public class IndexModel(IInvoiceService invoiceService) : PageModel
{
    public List<InvoiceDetailsDto> Invoices { get; private set; } = [];

    [BindProperty(SupportsGet = true)]
    public string? StatusFilter { get; set; }

    public async Task OnGetAsync()
    {
        if (!string.IsNullOrWhiteSpace(StatusFilter) && Enum.TryParse<InvoiceStatus>(StatusFilter, out var status))
            Invoices = (await invoiceService.GetInvoicesByStatusAsync(status)).ToList();
        else
            Invoices = (await invoiceService.GetAllInvoicesAsync()).ToList();
    }
}
