using ClinicSystem.Application.Dtos;
using ClinicSystem.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicSystem.Web.Pages.Billing;

[Authorize(Roles = "Admin,Receptionist")]
public class DetailsModel(IInvoiceService invoiceService) : PageModel
{
    public InvoiceDetailsDto? Invoice { get; private set; }
    public string? PaymentError { get; private set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Invoice = await invoiceService.GetInvoiceByIdAsync(id);
        return Invoice is null ? NotFound() : Page();
    }

    public async Task<IActionResult> OnPostAddPaymentAsync(Guid id, decimal amount, string method)
    {
        try
        {
            await invoiceService.AddPaymentAsync(id, new PaymentCreateDto(amount, method));
            TempData["Success"] = $"Payment of E{amount:F2} recorded successfully.";
            return RedirectToPage(new { id });
        }
        catch (KeyNotFoundException ex)
        {
            Invoice = await invoiceService.GetInvoiceByIdAsync(id);
            PaymentError = ex.Message;
            return Page();
        }
        catch (InvalidOperationException ex)
        {
            Invoice = await invoiceService.GetInvoiceByIdAsync(id);
            PaymentError = ex.Message;
            return Page();
        }
        catch (ArgumentException ex)
        {
            Invoice = await invoiceService.GetInvoiceByIdAsync(id);
            PaymentError = ex.Message;
            return Page();
        }
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        try
        {
            await invoiceService.DeleteInvoiceAsync(id);
            TempData["Success"] = "Invoice deleted.";
            return RedirectToPage("Index");
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToPage(new { id });
        }
    }
}
