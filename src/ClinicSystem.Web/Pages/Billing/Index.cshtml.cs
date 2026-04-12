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

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;
    
    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 10;
    
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }

    public async Task OnGetAsync()
    {
        var page = Math.Max(1, PageNumber);
        var pageSize = Math.Max(1, Math.Min(100, PageSize));
        
        IEnumerable<InvoiceDetailsDto> filteredInvoices;
        
        if (!string.IsNullOrWhiteSpace(StatusFilter) && Enum.TryParse<InvoiceStatus>(StatusFilter, out var status))
            filteredInvoices = await invoiceService.GetInvoicesByStatusAsync(status);
        else
            filteredInvoices = await invoiceService.GetAllInvoicesAsync();
        
        TotalCount = filteredInvoices.Count();
        TotalPages = (TotalCount + pageSize - 1) / pageSize;
        
        Invoices = filteredInvoices
            .OrderByDescending(i => i.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        
        PageNumber = page;
        PageSize = pageSize;
    }
}
